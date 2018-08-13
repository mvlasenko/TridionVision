<html id="DocumentOptionsPopup" class="apopup" xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Search Images</title>
        <link rel='shortcut icon' type='image/x-icon' href='${ImgUrl}favicon.png' />
    </head>
    <body id="StackElement" style="height: 580px;">
        <table border="0" style="table-layout: fixed; width:90%; height:90%;">
            <tr id="rowHeader">
                <td>
                    <span id="report-options-header">Search Images</span>
                </td>
            </tr>
            <tr id="rowLabel">
                <td>
                    <div style="display: inline-block; vertical-align: top; padding-left: 10px;">
                        Name of object:
                    </div>
                    <input id="word" style="display: inline-block; vertical-align: top" />
                    <div style="display: inline-block; vertical-align: top">
                        <c:button id="BtnSearch" runat="server" tabindex="1" label="Search"></c:button>
                    </div>
                </td>
            </tr>
            <tr id="rowTable">
                <td id="itemTypes">

                    <div class="tab-body active">
                        <progress id="progBar"></progress>
                    </div>

                </td> 
            </tr>
            <tr id="FooterRow">
                <td>
                    <div style="padding-left: 10px;">
                        <c:button id="BtnOk" runat="server" tabindex="2" label="OK"></c:button>
                    </div>
                </td>
            </tr>
        </table>
    </body>
</html>