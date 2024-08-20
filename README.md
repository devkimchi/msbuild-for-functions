# MSBuild for Azure Functions on Containers

As a .NET developer, when you build a container image for your [Azure Functions](https://learn.microsoft.com/azure/azure-functions/functions-overview?WT.mc_id=dotnet-147942-juyoo) app, you can use the `Dockerfile` to define the container image. However, you can also use the `dotnet publish` command to build and publish the container image without a `Dockerfile`. This repository provides sample .NET function apps using container images with `Dockerfile` and with `dotnet publish`.

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download/dotnet/8.0?WT.mc_id=dotnet-147942-juyoo) with [.NET Aspire workload](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling?WT.mc_id=dotnet-147942-juyoo)
- [Visual Studio](https://visualstudio.microsoft.com/?WT.mc_id=dotnet-147942-juyoo) or [Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=dotnet-147942-juyoo) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) + [Azure Functions](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions?WT.mc_id=dotnet-147942-juyoo)
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local?WT.mc_id=dotnet-147942-juyoo)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Getting Started

> **NOTE**: Make sure Docker Desktop is running before you start.

### Run with `Dockerfile`

1. Run `func init` to create a new Azure Functions app with Docker support.

    ```bash
    func init FunctionAppWithDockerfile --worker-runtime dotnet-isolated --docker --target-framework net8.0
    ```

1. Confirm the `Dockerfile` is created in the `FunctionAppWithDockerfile` folder.
1. Run `func new` to add a new HTTP trigger function.

    ```bash
    pushd ./FunctionAppWithDockerfile
    func new -n HttpExampleTrigger -t HttpTrigger --a anonymous
    ```

1. Open `HttpExampleTrigger.cs` and modify the line.

    ```csharp
    // Before
    return new OkObjectResult("Welcome to Azure Functions!");

    // After
    return new OkObjectResult("Welcome to Azure Functions with Dockerfile!");
    ```

1. Run `docker build` to build the container image.

    ```bash
    docker build . -t funcapp:latest-dockerfile
    ```

1. Return to the repository root.

    ```bash
    popd
    ```

1. Run the function app container.

    ```bash
    docker run -d -p 7071:80 --name funcappdockerfile funcapp:latest-dockerfile
    ```

1. Open the browser and navigate to `http://localhost:7071/api/HttpExampleTrigger` to see the function app running.
1. Run the following command to stop and remove the container.

    ```bash
    docker stop funcappdockerfile
    docker rm funcappdockerfile
    ```

### Run with `dotnet publish`

1. Run `func init` to create a new Azure Functions app without Docker support.

    ```bash
    func init FunctionAppWithMSBuild --worker-runtime dotnet-isolated --target-framework net8.0
    ```

1. Run `func new` to add a new HTTP trigger function.

    ```bash
    pushd ./FunctionAppWithMSBuild
    func new -n HttpExampleTrigger -t HttpTrigger --a anonymous
    ```

1. Open `HttpExampleTrigger.cs` and modify the line.

    ```csharp
    // Before
    return new OkObjectResult("Welcome to Azure Functions!");

    // After
    return new OkObjectResult("Welcome to Azure Functions with MSBuild!");
    ```

1. Open `FunctionAppWithMSBuild.csproj` and add the following item group nodes.

    ```xml
    <ItemGroup>
      <ContainerEnvironmentVariable Include="AzureWebJobsScriptRoot" Value="/home/site/wwwroot" />
      <ContainerEnvironmentVariable Include="AzureFunctionsJobHost__Logging__Console__IsEnabled" Value="true" />
    </ItemGroup>

    <ItemGroup Label="ContainerAppCommand Assignment">
      <ContainerAppCommand Include="/opt/startup/start_nonappservice.sh" />
    </ItemGroup>
    ```

1. Return to the repository root.

    ```bash
    popd
    ```

1. Run the following `dotnet publish` command to build and publish the function app.

    ```bash
    dotnet publish ./FunctionAppWithMSBuild `
        -t:PublishContainer `
        --os linux --arch x64 `
        -p:ContainerBaseImage=mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 `
        -p:ContainerRepository=funcapp `
        -p:ContainerImageTag=latest-msbuild `
        -p:ContainerWorkingDirectory="/home/site/wwwroot"
    ```

1. Run the function app container.

    ```bash
    docker run -d -p 7071:80 --name funcappmsbuild funcapp:latest-msbuild
    ```

1. Open the browser and navigate to `http://localhost:7071/api/HttpExampleTrigger` to see the function app running.
1. Run the following command to stop and remove the container.

    ```bash
    docker stop funcappmsbuild
    docker rm funcappmsbuild
    ```
