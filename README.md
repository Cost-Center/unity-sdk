# Cost Center Unity SDK
Cost Center Unity SDK is a close source attribution for Unity games using Firebase Analytics.

## Getting Started
Requirement [Firebase Unity SDK](https://firebase.google.com/docs/unity/setup):
-   Firebase Analytics

## Installation

#### Using Git (for Unity 2018.3 or later):
Find the manifest.json file in the Packages folder of your project and edit it to look like this:

    {
        "dependencies": {
            "costcenter-sdk": "https://github.com/Cost-Center/unity-sdk.git?path=/Assets/CostCenter#{version}",
            ...
        },
    }
To update the package, change suffix  `#{version}`  to the target version.
-   e.g.  `"https://github.com/Cost-Center/unity-sdk.git?path=/Assets/CostCenter#v1.2.0"`

#### For Unity 2018.2 or earlier
1.  Download a source code zip file from  [Releases](https://github.com/Cost-Center/unity-sdk/releases)  page
2.  Extract it
3.  Import it into the following directory in your Unity project

## How to use
 - Select `GameObject/Cost Center/Create Manager`  from Unity menu
 - A new manager game object will be added to current `Scene`.
 - **Important**: there should be only one game object for Cost Center for all scences. If a project has multiple scenes, you can create this game object once only at the loading scene, then tick "Don't Destroy"
 - Select the new game object, click `Add Attribution` in CCServices
 - MMP: Import `Adapters/cc_adapter_<attribution>_v<x.y.z>.unitypackage` to your Unity project

## License
-   ExtraLabs

## See Also
-   Git page :  [https://github.com/Cost-Center/unity-sdk](https://github.com/Cost-Center/unity-sdk)
-   Releases :  [https://github.com/Cost-Center/unity-sdk/releases](https://github.com/Cost-Center/unity-sdk/releases)
-   Change log :  [https://github.com/Cost-Center/unity-sdk/blob/master/CHANGELOG.md](https://github.com/Cost-Center/unity-sdk/blob/master/CHANGELOG.md)
