#region Header
// Revit API .NET Labs
//
// Copyright (C) 2007-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software
// for any purpose and without fee is hereby granted, provided
// that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
#endregion // Header

#region Namespaces
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

using DesignAutomationFramework;

#endregion // Namespaces

namespace ExportToPdfsApp
{


    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ExportToPdfsApp : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnStartup(ControlledApplication app)
        {
            DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnShutdown(ControlledApplication app)
        {
            return ExternalDBApplicationResult.Succeeded;
        }

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            e.Succeeded = ExportToPdfs(e.DesignAutomationData);
        }

        public static bool ExportToPdfs(DesignAutomationData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Application rvtApp = data.RevitApp;
            if (rvtApp == null)
                throw new InvalidDataException(nameof(rvtApp));

            string modelPath = data.FilePath;
            if (String.IsNullOrWhiteSpace(modelPath))
                throw new InvalidDataException(nameof(modelPath));

            Document doc = data.RevitDoc;
            if (doc == null)
                throw new InvalidOperationException("Could not open document.");

            InputParams inputParams = InputParams.Parse("params.json");

            return ExportToPdfsImp(rvtApp, doc, inputParams);
        }


        private static bool ExportToPdfsImp(Application rvtApp, Document doc, InputParams inputParams)          
        {

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Export PDF");

                List<View> views = new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(vw => (
                    inputParams.DrawingSheet && vw.ViewType == ViewType.DrawingSheet && !vw.IsTemplate
                    || inputParams.ScheduleTable && vw.ViewType == ViewType.DraftingView
                        )
                    ).ToList();

                Console.WriteLine("the number of views: " + views.Count);

                IList<ElementId> viewIds = new List<ElementId>();
                foreach (View view in views)
                {
                    //ViewSheet viewSheet = view as ViewSheet;
                    Console.WriteLine(view.Name);
                    viewIds.Add(view.Id);
                }


                if (0 < views.Count)
                {
                    PDFExportOptions options = new PDFExportOptions();
                    options.FileName = "result";
                    options.Combine = true;
                    string workingFolder = Directory.GetCurrentDirectory();
                    doc.Export(workingFolder, viewIds, options);
                }
                tx.RollBack();
            }
            return true;
        }

    }


    /// <summary>
    /// InputParams is used to parse the input Json parameters
    /// </summary>
    internal class InputParams
    {
        public bool DrawingSheet { get; set; } = true;
        public bool ScheduleTable { get; set; } = true;
        static public InputParams Parse(string jsonPath)
        {
            try
            {
                if (!File.Exists(jsonPath))
                    return new InputParams { DrawingSheet = true, ScheduleTable = true };

                string jsonContents = File.ReadAllText(jsonPath);
                return JsonConvert.DeserializeObject<InputParams>(jsonContents);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception when parsing json file: " + ex);
                return null;
            }
        }
    }

}
