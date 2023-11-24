<h1>DEPRECATED</h1>
This repository has been deprecated and all its content was moved to the new one located here https://github.com/autodesk-platform-services/aps-export-revit-pdf


# forge-revit2pdf

[![Node.js](https://img.shields.io/badge/Node.js-8.0-blue.svg)](https://nodejs.org/)
[![npm](https://img.shields.io/badge/npm-4.0-blue.svg)](https://www.npmjs.com/)
![Platforms](https://img.shields.io/badge/Web-Windows%20%7C%20MacOS%20%7C%20Linux-lightgray.svg)
[![Data-Management](https://img.shields.io/badge/Data%20Management-v1-green.svg)](http://developer.autodesk.com/)
[![Design-Automation](https://img.shields.io/badge/Design%20Automation-v3-green.svg)](http://developer.autodesk.com/)
[![Forge-Viewer](https://img.shields.io/badge/Forge%20Viewer-v7-green.svg)](http://developer.autodesk.com/)


![Windows](https://img.shields.io/badge/Plugins-Windows-lightgrey.svg)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.7-blue.svg)
[![Revit-2022](https://img.shields.io/badge/Revit-2022-lightgrey.svg)](http://autodesk.com/revit)


![Advanced](https://img.shields.io/badge/Level-Advanced-red.svg)
[![MIT](https://img.shields.io/badge/License-MIT-blue.svg)](http://opensource.org/licenses/MIT)

# Description
This sample demonstrates how to export views and sheets from Revit to PDF with the support of Revit Design Automation Engine 2022. Check the [blog](https://forge.autodesk.com/blog/design-automation-revit-2022-now-support-exporting-pdf-directly) for more information. 

# Thumbnail
![thumbnail](/thumbnail.png)

# Live Demo
[https://revit2pdf.herokuapp.com/](https://revit2pdf.herokuapp.com/)

# Web App Setup

## Prerequisites

1. **Forge Account**: Learn how to create a Forge Account, activate subscription and create an app at [this tutorial](http://learnforge.autodesk.io/#/account/). 
2. **Visual Code**: Visual Code (Windows or MacOS).
3. **Visual Studio 2019** (Windows).
4. **ngrok**: Routing tool, [download here](https://ngrok.com/)
5. **Revit 2022**: required to compile changes into the plugin
6. **JavaScript ES6** syntax for server-side.
7. **JavaScript** basic knowledge with **jQuery**


For using this sample, you need an Autodesk developer credentials. Visit the [Forge Developer Portal](https://developer.autodesk.com), sign up for an account, then [create an app](https://developer.autodesk.com/myapps/create). For this new app, use **http://localhost:3000/api/forge/callback/oauth** as Callback URL, although is not used on 2-legged flow. Finally take note of the **Client ID** and **Client Secret**.

## Running locally
1. Install [NodeJS](https://nodejs.org), version 8 or newer.

2. Clone this project or download it (this `main` branch only). It's recommended to install [GitHub desktop](https://desktop.github.com/). To clone it via command line, use the following (**Terminal** on MacOSX/Linux, **Git Shell** on Windows):

    git clone https://github.com/Autodesk-Forge/forge-revit2pdf

3. (Optional) Open ExportToPdfsApp.sln in Visual Studio, build the Revit Addon, it will build `ExportToPdfsApp.dll` and package `ExportToPdfsApp.bundle` into folder of \</public/bundles\> automatically, or you can use the build-in package of `ExportToPdfsApp.zip`. 

4. Install the required packages using `npm install`.
5. Start server by `npm start`.
6. Open the App, click `Configure` button to create the AppBundle & Activity before exporting PDF file, please check the video for the steps at [https://youtu.be/1NCeH7acIko](https://youtu.be/1NCeH7acIko). You can also delete the existing AppBundle & Activity and re-create with different Design Automation Revit engine version.

### ngrok

Run `ngrok http 3000` to create a tunnel to your local machine, then copy the address into the `FORGE_WEBHOOK_URL` environment variable. Please check [WebHooks](https://forge.autodesk.com/en/docs/webhooks/v1/tutorials/configuring-your-server/) for details.

### Environment variables

Set the environment variables with your client ID & secret and finally start it. Via command line, navigate to the folder where this repository was cloned and use the following:

Mac OSX/Linux (Terminal)

    npm install
    export FORGE_CLIENT_ID=<<YOUR CLIENT ID FROM DEVELOPER PORTAL>>
    export FORGE_CLIENT_SECRET=<<YOUR CLIENT SECRET>>
    export FORGE_CALLBACK_URL=<<YOUR CALLBACK URL>>
    export FORGE_WEBHOOK_URL=<<YOUR DESIGN AUTOMATION FOR REVIT CALLBACK URL>>
    npm start

Windows (use **Node.js command line** from Start menu)

    npm install
    set FORGE_CLIENT_ID=<<YOUR CLIENT ID FROM DEVELOPER PORTAL>>
    set FORGE_CLIENT_SECRET=<<YOUR CLIENT SECRET>>
    set FORGE_CALLBACK_URL=<<YOUR CALLBACK URL>>
    set FORGE_WEBHOOK_URL=<<YOUR DESIGN AUTOMATION FOR REVIT CALLBACK URL>>
    npm start

Windows (use **PowerShell** from Start menu)

    npm install
    $env:FORGE_CLIENT_ID="YOUR CLIENT ID FROM DEVELOPER PORTAL"
    $env:FORGE_CLIENT_SECRET="YOUR CLIENT SECRET"
    $env:FORGE_CALLBACK_URL="YOUR CALLBACK URL"
    $env:FORGE_WEBHOOK_URL="YOUR DESIGN AUTOMATION FOR REVIT CALLBACK URL"
    npm start

**Note.**
environment variable examples:
- FORGE_CALLBACK_URL: `http://localhost:3000/api/forge/callback/oauth`
- FORGE_WEBHOOK_URL: `http://808efcdc123456.ngrok.io/api/forge/callback/designautomation`

Optional:
- DESIGN_AUTOMATION_NICKNAME: Your client id will be used by default, but if you have set up the [nickname](https://forge.autodesk.com/en/docs/design-automation/v3/tutorials/revit/step3-create-nickname/), please specify your nickname to this environment variable.
- DESIGN_AUTOMATION_ACTIVITY_NAME: `ExportToPdfsAppActivity` will be used as default activity name after creating the bundle and activity, please specify the value if restarting the server. 

### Using the app

Open the browser: [http://localhost:3000](http://localhost:3000), it provides the abilities to export pdf file for views|sheets: 

1. (Optional)Make sure the AppBundle & Activity are created. If not, click `Configure` button to create the AppBundle & Activity, please check the video for the steps at [https://youtu.be/1NCeH7acIko](https://youtu.be/1NCeH7acIko). You can also delete the existing AppBundle & Activity and re-create with different Design Automation Revit engine version.

2. Select Revit file version in BIM360 Hub to view the Model, Select view types which you want to export, click 'Export'.

3. When the pdf file is successfully exported, click `DOWNLOAD` to download the file and open.


## Deployment

To deploy this application to Heroku, the **Callback URL** for Forge must use your `.herokuapp.com` address. After clicking on the button below, at the Heroku Create New App page, set your Client ID, Secret, Callback URL and Revit Design Automation variables for Forge.

[![Deploy](https://www.herokucdn.com/deploy/button.svg)](https://heroku.com/deploy?template=https://github.com/Autodesk-Forge/forge-revit2pdf)

Watch [this video](https://www.youtube.com/watch?v=Oqa9O20Gj0c) on how deploy samples to Heroku.

## Packages used

The [Autodesk Forge](https://www.npmjs.com/package/forge-apis) packages is included by default. Some other non-Autodesk packaged are used, including [socket.io](https://www.npmjs.com/package/socket.io), [express](https://www.npmjs.com/package/express).

## Further Reading

Documentation:
- This sample is based on [Learn Forge Tutorial](https://github.com/Autodesk-Forge/learn.forge.viewhubmodels/tree/nodejs), please check details there about the basic framework if you are not familar. 

- [Design Automation API](https://forge.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/)
- [BIM 360 API](https://developer.autodesk.com/en/docs/bim360/v1/overview/) and [App Provisioning](https://forge.autodesk.com/blog/bim-360-docs-provisioning-forge-apps)
- [Data Management API](httqqqps://developer.autodesk.com/en/docs/data/v2/overview/)

Desktop APIs:

- [Revit](https://knowledge.autodesk.com/support/revit-products/learn-explore/caas/simplecontent/content/my-first-revit-plug-overview.html)

## Troubleshooting

After installing Github desktop for Windows, on the Git Shell, if you see a ***error setting certificate verify locations*** error, use the following:

    git config --global http.sslverify "false"

## Tips & Tricks
- Before using the sample to call the workitem, you need to setup your Appbundle & Activity of Design Automation, you can simply use the `Configure` button in the Web Application to create the Appbundle & Activity([https://youtu.be/1NCeH7acIko](https://youtu.be/1NCeH7acIko)). 

- In this sample, we set the maximum number of views to export to 5 for demonstration purpose. You can modify or remove the value `Max_views` as required at this line https://github.com/Autodesk-Forge/forge-revit2pdf/blob/main/ExportToPdfsPlugin/ExportToPdfs.cs#L113.

- If there is no view to be exported, an empty pdf file will be generated.

## Limitation
- Currently Revit Cloud Worksharing is not supported by the Design Automation.  The scenario that this sample demonstrates is applicable only with a file-based Revit model. 
- Client JavaScript requires modern browser.
- The sample only supports Revit Design Automation Engine 2022.

## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.

## Written by

Zhong Wu [@johnonsoftware](https://twitter.com/johnonsoftware), [Forge Partner Development](http://forge.autodesk.com)
