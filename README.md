# dotnet-linux-imagediff
Calculating changes to images taken using a webcam on Linux.

# Requirements

This API requires fswebcam and imagemagick to be installed. Dockerfile example:

```Dockerfile
FROM microsoft/dotnet:2.0.0-runtime-stretch-arm32v7
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends fswebcam imagemagick imagemagick-doc
```

# How it works

It takes pictures with the attached usb webcams, so make sure your kernel has those drivers installed. You should see /dev/video0, /dev/video1, etc. on your file system when the cameras are plugged in.

Pictures are taken one by one, the assumption is that there is not enough power to run multiple cameras on the same usb hub.

# Known Limitations

It is sloooooow... Any suggestions on how to speed up the process would be welcomed! I may be looking into using a native library instead of calling out to fswebcam via the shell. It would also be nice to change the resolution, frames skipped, etc. It's on the todo list... :)

