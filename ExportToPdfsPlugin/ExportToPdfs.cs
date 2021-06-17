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

        public bool ExportToPdfs(DesignAutomationData data)
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


        private bool ExportToPdfsImp(Application rvtApp, Document doc, InputParams inputParams)          
        {

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Export PDF");

                List<View> views = new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Where(vw => (
                    !vw.IsTemplate && vw.CanBePrinted
                    && (inputParams.DrawingSheet && vw.ViewType == ViewType.DrawingSheet
                    || inputParams.ThreeD    && vw.ViewType == ViewType.ThreeD
                    || inputParams.Detail    && vw.ViewType == ViewType.Detail
                    || inputParams.Elevation && vw.ViewType == ViewType.Elevation
                    || inputParams.FloorPlan && vw.ViewType == ViewType.FloorPlan
                    || inputParams.Rendering && vw.ViewType == ViewType.Rendering
                    || inputParams.Section   && vw.ViewType == ViewType.Section )
                    )
                ).ToList();

                Console.WriteLine("the number of views: " + views.Count);

                // Note: Setting the maximum number of views to be exported as 5 for demonstration purpose.
                // Remove or edit here in your production application
                const int Max_views = 5;
                IList<ElementId> viewIds = new List<ElementId>();
                for(int i = 0; i < views.Count && i < Max_views; ++i)  // To Do: edit or remove max_views as required.
                {
                    Console.WriteLine(views[i].Name + @", view type is: " + views[i].ViewType.ToString());
                    viewIds.Add(views[i].Id);
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
        public bool ThreeD { get; set; } = true;
        public bool Detail { get; set; } = true;
        public bool Elevation { get; set; } = true;
        public bool FloorPlan { get; set; } = true;
        public bool Section { get; set; } = true;
        public bool Rendering { get; set; } = true;

        static public InputParams Parse(string jsonPath)
        {
            try
            {
                if (!File.Exists(jsonPath))
                    return new InputParams { DrawingSheet = true, ThreeD = true, Detail = true, Elevation = true, FloorPlan = true, Section = true, Rendering = true };

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
