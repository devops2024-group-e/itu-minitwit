# What is being changed?
> **Date:** 10/02/24
>
> **Author** Sarah Christiansen

The task is to convert the code from flask to Razor Pages. The refactoring should strive to be as similar to the source code as possible.

## What did we do?
1. We created the 'Shared' folder under 'Pages' to hold UI that we wish to use across different pages. Here we then added `_Layout.cshtml` and rewrote the code in `layout.html` from the python souce code to match the correct format of '.cshtml' files to encompass the same UI format.

2. We added the `style.css` file from the original project and added it to the 'wwwroot' folder under the name `main.css`. This was done to connect the html and css properly, so that the refactored version of the website now matched the original one in terms of looks.

## What is missing?
1. We need to figure out if the flash message in the original source code are already encompassed by the error messages or if they need to be added another way.

2. We need to figure out how to check if a user is logged in or not in order to showcase different links in the header.