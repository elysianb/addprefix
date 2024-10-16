﻿# Installation

- Install Dotnet Framework 4.8

https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer

- Extract binaries from a release into a local directory. Then, use it

`addprefix --help` 

# Documentation

NAME

        addprefix - add prefix to media file based on date taken metadata or 
        file creation date

SYNOPSIS

        addprefix [options]

DESCRIPTION

        addprefix is a tool for rename media file by adding a prefix based on
        a timestamp. If the file still starts with the timestamp, it is not
        renamed.

OPTIONS

        Options start with one or two dashes. Many of the options require an 
        additional value next to them.

        Options requires a space between it and its value.

        -p, --path
                Specify working directory for rename operation. A preview 
                execution is always processed before the rename operation. See
                --preview for preview operation details.

                Example:
                  addprefix --path .
                  addprefix -p . 

                Additional options are the following:

                -e, --extensions
                  file extensions to proceed.
                  default value is .jpg,.jpeg,.png,.webp,.gif,.mp4,.avi,.mpg,.mpeg,.mov,.mkv

                  Example:
                    addprefix -p . -e .jpg,.jpeg
        
                -f, --format
                  specify date fomrmating.
                  default value is yyyyMMdd-HHmmss

                  Example:
                    addprefix -p . -f yyyyMMdd
        
                -P, --preview      
                  preview mode. Default value is false.

                  The output of preview mode is a script runnable with
                  bash or powershell for renaming media files and get infos
                  on number of files to rename and processed.

                  Example:
                    addprefix -p . -P -e .jpg,.jpeg

                    # --- Example output ---
                    # Processing folder   : .
                    # Extensions          : .jpg,.jpeg
                    # Format              : yyyMMdd-HHmmss
                    # Include sub folders : no
                    # Preview Mode        : yes
                    # File(s) found       : 3
                    # Pre-processing...
                    mv .\media-file1.jpeg .\20241006-131439-media-file1.jpeg
                    mv .\media-file2.jpeg .\20241006-131439-media-file2.jpeg
                    mv .\media-file3.jpeg .\20241006-131440-media-file3.jpeg
                    # 3 file(s) to rename
                    # --- End of example output ---
        
                -r, --recursive
                  also process sub directories
                  default value is false

                  Example:
                    addprefix -p . -r

        -h, --help
                Display this manual page

        -v, --version
                Display the current version number
