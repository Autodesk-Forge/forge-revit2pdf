/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

$(document).ready(function () {

    $('#startWorkitem').click(export2Pdf);
    $('#cancelBtn').click(async function(){
        if (workingItem != null) {
            try {
                await cancelWorkitem(workingItem);
                console.log('The job is cancelled');
            } catch (err) {
                console.log('Failed to cancel the job');
            }
        }
    });
});

var sourceNode  = null;
var workingItem = null;
var inputExcel  = null;
var exporting   = true;
var _fileInputForm = null;


const SOCKET_TOPIC_WORKITEM = 'Workitem-Notification';
// const BUCKET_KEY = 'revitiosamplebyzhong';

socketio = io();
socketio.on(SOCKET_TOPIC_WORKITEM, (data)=>{
  if(workingItem === null || data.WorkitemId !== workingItem)
    return;
    
  const status = data.Status.toLowerCase();
  updateStatus( status, data.ExtraInfo );
  
  // enable the create button and refresh the hubs when completed/failed/cancelled
  if(status === 'completed' || status === 'failed' || status === 'cancelled'){
    workingItem = null;
  }
  if(status === 'completed' && sourceNode != null){
    console.log('Parameters are handled');
    console.log(data);
    sourceNode = null;
  }
})




async function export2Pdf() {
    const instanceTree = $('#sourceHubs').jstree(true);
    if( instanceTree == null ){
        alert('Can not get the user hub');
        return;
    }

    sourceNode = instanceTree.get_selected(true)[0];
    // use == here because sourceNode may be undefined or null
    if (sourceNode == null || sourceNode.type !== 'versions' ) {
        alert('Can not get the selected file, please make sure you select a version as input');
        return;
    }

    const fileName = instanceTree.get_text(sourceNode.parent);
    const fileNameParams = fileName.split('.');
    if( fileNameParams[fileNameParams.length-1].toLowerCase() !== "rvt"){
        alert('please select Revit project and try again');
        return;
    }

    if( sourceNode.original.storage == null){
        alert('Can not get the storage of the version');
        return;
    }

    const drawingSheet = $('#drawingSheet')[0].checked;
    const threeD    = $('#threeD')[0].checked;
    const detail    = $('#detail')[0].checked;
    const elevation = $('#elevation')[0].checked;
    const floorPlan = $('#floorPlan')[0].checked;
    const section   = $('#section')[0].checked;
    const rendering = $('#rendering')[0].checked;

    if( !drawingSheet && !threeD && !detail && !elevation && !floorPlan && !section && !rendering){
        alert('Please at least select one view type you want to export');
        return;
    }

    const inputJson = { 
        DrawingSheet : drawingSheet,
        ThreeD: threeD,
        Detail: detail,
        Elevation:elevation,
        FloorPlan:floorPlan,
        Section:section,
        Rendering:rendering
      };

      
    try {
        let res = null;
        updateStatus('started');
        res = await exportToPdfs(sourceNode.original.storage, inputJson);
        console.log('The pdf file is exported');
        console.log(res);
        workingItem = res.workItemId;
        updateStatus(res.workItemStatus);
    } catch (err) {
        console.log('Failed to export the pdf file');
        updateStatus('failed');
    }
    
    return;
}


async function exportToPdfs( inputRvt, inputJson){
    let def = $.Deferred();
  
    jQuery.get({
        url: '/api/forge/da4revit/v1/revit/' + encodeURIComponent(inputRvt) + '/pdfs',
        contentType: 'application/json', // The data type was sent
        dataType: 'json', // The data type will be received
        data: inputJson,
        success: function (res) {
            def.resolve(res);
        },
        error: function (err) {
            def.reject(err);
        }
    });

    return def.promise();
}





function cancelWorkitem( workitemId ){
    let def = $.Deferred();
  
    if(workitemId === null || workitemId === ''){
      def.reject("parameters are not correct.");  
      return def.promise();
    }
  
    $.ajax({
      url: '/api/forge/da4revit/v1/revit/' + encodeURIComponent(workitemId),
      type: "delete",
      dataType: "json",
      success: function (res) {
        def.resolve(res);
      },
      error: function (err) {
        def.reject(err);
      }
    });
    return def.promise();
  }
  
  
  function getWorkitemStatus( workitemId ){
    let def = $.Deferred();
  
    if(workitemId === null || workitemId === ''){
      def.reject("parameters are not correct.");  
      return def.promise();
    }
  
    jQuery.get({
      url: '/api/forge/da4revit/v1/revit/' + encodeURIComponent(workitemId),
      dataType: 'json',
      success: function (res) {
        def.resolve(res);
      },
      error: function (err) {
        console.log(err)
        def.reject(err);
      }
    });
    return def.promise();
  }


function updateStatus(status, extraInfo = '') {
    let statusText = document.getElementById('statusText');
    let upgradeBtnElm = document.getElementById('startWorkitem');
    let cancelBtnElm = document.getElementById('cancelBtn');
    switch (status) {
        case "started":
            setProgress(20, 'parametersUpdateProgressBar');
            statusText.innerHTML = "<h4>Step 1/3:  Uploading view type information</h4>"
            // Disable Create and Cancel button
            upgradeBtnElm.disabled = true;
            cancelBtnElm.disabled = true;
            break;
        case "pending":
            setProgress(40, 'parametersUpdateProgressBar');
            statusText.innerHTML = "<h4>Step 2/3: Running Design Automation</h4>"
            upgradeBtnElm.disabled = true;
            cancelBtnElm.disabled = false;
            break;
        case "success":
            setProgress(80, 'parametersUpdateProgressBar');
            statusText.innerHTML = "<h4>Step 3/4: Creating a new version</h4>"
            upgradeBtnElm.disabled = true;
            cancelBtnElm.disabled = true;
            break;
        case "completed":
            setProgress(100, 'parametersUpdateProgressBar');
            statusText.innerHTML = "<h4>Step 3/3: Done, Ready to <a href='" + extraInfo + "'>DOWNLOAD</a></h4>";
            // Enable Create and Cancel button
            upgradeBtnElm.disabled = false;
            cancelBtnElm.disabled = true;
            break;
        case "failed":
            setProgress(0, 'parametersUpdateProgressBar');
            statusText.innerHTML = "<h4>Failed to process Excel</h4>"
            // Enable Create and Cancel button
            upgradeBtnElm.disabled = false;
            cancelBtnElm.disabled = true;
            break;
        case "cancelled":
            setProgress(0, 'parametersUpdateProgressBar');
            statusText.innerHTML = "<h4>The operation is cancelled</h4>"
            // Enable Create and Cancel button
            upgradeBtnElm.disabled = false;
            cancelBtnElm.disabled = true;
            break;
    }
}


function setProgress(percent, progressbarId ) {
    let progressBar = document.getElementById(progressbarId);
    progressBar.style = "width: " + percent + "%;";
    if (percent === 100) {
        progressBar.parentElement.className = "progress progress-striped"
    } else {
        progressBar.parentElement.className = "progress progress-striped active"
    }
}
