## Notes on how or why the current implementation was chosen

replace file names with guids for security. 
Image medata is saved in memory in this prototype (imagemetadata usually is saved into db)
I'm using buffered memory for uploaded images in this prototype, there could be many ways: streaming, background processing raw data...
Different uploaded image services could be added to replace the images validation and handling (create new service and change DI)


## Current Limits
   1. Individual File Size: I've hardcoded a check for 10MB per file in the controller.
   2. Total Request Size (ASP.NET Core Default):
       * By default, ASP.NET Core limits the total multipart body size to approx. 128MB.
       * If you try to upload 20 images of 10MB each (200MB total), the server will reject the request before it even reaches my code.
   3. Timeout Limits: Large batch uploads can trigger browser or server timeouts (usually 2 minutes) if the network is slow, causing the entire batch to
      fail.
   4. Memory Pressure: While IFormFile is efficient (it streams to disk for large files), processing a massive List<IFormFile> still consumes server
      resources during the request lifecycle.
   5. User Experience: In a single POST with multiple files, the user gets no progress feedback for individual files; it's an "all or nothing" operation.



## Not included in this prototype:

No docker setup - make it simple to just download and run
no ci/cd pipeline setup
no advanced image validation - we can add image file signature check or integrate with virus saanning service
no caching - we could implement redis for  composition images in upload package
