# Image Layout Composer

A full-stack solution built with ASP.NET Core Web API and Blazor WebAssembly to compose images into various grid layouts.

## Features

- **Multi-image Upload**: Upload multiple PNG/JPG files.
- **Grid Layouts**: Support for 2x2, 3x3, and 4x4 grids.
- **Smart Resizing**: Automatically fits images into grid cells while preserving aspect ratio.
- **Downloadable Results**: Preview and download the final composed image.

## Project Structure

- `src/ImageLayoutComposer.Api`: ASP.NET Core Web API handling storage and image processing.
- `src/ImageLayoutComposer.Client`: Blazor WebAssembly frontend.
- `src/ImageLayoutComposer.Shared`: Common models and DTOs.
- `tests/ImageLayoutComposer.Tests`: Unit tests for core logic.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

## Setup and Run

### 1. Run the API

Open a terminal in the project root:

```bash
cd src/ImageLayoutComposer.Api
dotnet run --launch-profile https
```

The API will start at `https://localhost:7120` (default for the profile).

### 2. Run the Client

Open another terminal in the project root:

```bash
cd src/ImageLayoutComposer.Client
dotnet run
```

The Blazor application will be available at the URL shown in the output (usually `http://localhost:5xxx` or `https://localhost:7xxx`).

**Note**: Ensure the API is running first as the Client is configured to point to `https://localhost:7120`.

## API Usage

### Upload Images
- **Endpoint**: `POST /api/images/upload`
- **Body**: `MultipartFormDataContent` (form files)
- **Returns**: List of image metadata including IDs.

### Compose Grid
- **Endpoint**: `POST /api/layouts/compose`
- **Body**: 
  ```json
  {
    "layout": 0, // 0: 2x2, 1: 3x3, 2: 4x4
    "imageIds": ["guid1", "guid2", ...]
  }
  ```
- **Returns**: The composed image (JPEG).

## Assumptions and Tradeoffs

- **Storage**: Images are stored in the `src/ImageLayoutComposer.Api/uploads` directory. Metadata is kept in-memory for the duration of the API session.
- **Image Processing**: Used `SixLabors.ImageSharp` for high-performance, cross-platform image manipulation.
- **Validation**: Basic file size and type validation is implemented. In a production environment, more robust magic number verification and malware scanning would be recommended.
- **Security**: Uploaded files are treated as untrusted. Filenames are ignored in favor of generated GUIDs to prevent path traversal and collisions.

## Known Limitations

- In-memory metadata store means uploads are lost when the API restarts.
- Simple grid composition (no custom borders or padding).
- Fixed final image size (2048x2048).
