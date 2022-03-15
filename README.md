# sugcon-send

## About this Solution



## Support
The template output as provided is supported by Sitecore. Once changed or amended,
the solution becomes a custom implementation and is subject to limitations as
defined in Sitecore's [scope of support](https://kb.sitecore.net/articles/463549#ScopeOfSupport).

## Prerequisites
* Docker for Windows, with Windows Containers enabled

## Running this Solution
1. If your local IIS is listening on port 443, you'll need to stop it.
   > This requires an elevated PowerShell or command prompt.
   ```
   iisreset /stop
   ```

1. Initially setup certificates and hostnames (this needs to be with elevated mode aka. run as administrator)

    ```ps1
    .\init.ps1
    ```

1. After completing this environment preparation, run the startup script
   from the solution root:
    ```ps1
    .\up.ps1
    ```

1. When prompted, log into Sitecore via your browser, and
   accept the device authorization.

1. Wait for the startup script to open browser tabs for the rendered site
   and Sitecore Launchpad.

## Using the Solution
* A publish of the `Platform` project will update the running `cm` service.
* The running `rendering` service uses `dotnet watch` and will recompile
  automatically for any changes you make. You can also run the `Rendering`
  project directly from Visual Studio.
* Review README's found in the projects and throughout the solution
  for additional information.