# Auto-classify Images A4T Plugin

This plugin makes analysis of multimedia components and generates keywords automatically.
Generated keywords link to multimedia component via metadata.

Implementation
==============
Under the hood Google Vision service is used.<br />
https://cloud.google.com/vision/ <br />
Service takes binary data of image and returns a list of keywords that describe image.
Keywords can be generated for selected multimedia component or for all image components within the selected folder.
All generated keywords go to single category.

How to use
==========
1. Make sure that A4T plugin contains proper settings, category exists and multimedia schema contains "keywords" field.
2. Highlight single multimedia component or folder that recursively contains multimedia components.
3. Use "Auto-classify Images" button on the Ribbon bar or "Auto-classify  Images" context menu.
4. Press "Generate Keywords" button in modal window.
5. You can use modal window to access multimedia components.
6. Open any component that contains multimedia component link. Open dialog for selecting multimedia components. In this dialog you can use keywords to search necessary multimedia component.

Install
=======
1. Install plugin from https://www.alchemywebstore.com/ or from .a4t package.
2. Create new project in Google Console. Enable Cloud Vision API.<br />
   https://console.developers.google.com
3. Generate credentials json file.<br />
   https://console.developers.google.com 
4. Create Category in "Schemas" publication or above.
5. Add "Keywords" field to metadata of multimedia schema. Check "Values will be Selected from a List" and "Category". Select category. Select list type = "Checkboxes" 
6. Change plugin settings. Set Category and path to credentials json file.
7. Refresh browser and check out if "Auto-classify Images" button appeared.
