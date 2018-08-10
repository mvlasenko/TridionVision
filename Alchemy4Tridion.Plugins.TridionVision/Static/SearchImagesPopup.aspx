<html id="DocumentOptionsPopup" class="apopup" xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Search Images</title>
        <link rel='shortcut icon' type='image/x-icon' href='${ImgUrl}favicon.png' />
    </head>
    <body id="StackElement">
        <table border="0" cellpadding="3" cellspacing="0" style="table-layout: fixed; width:100%; height:100%;">
            <tr id="rowHeader">
                <td valign="top">
                    <span id="report-options-header">Search Images</span>
                </td>
            </tr>
            <tr id="rowLabel">
                <td valign="top">
                    Name of object:
                    <input id="word" />
                    <c:button id="BtnSearch" runat="server" tabindex="1" label="Search"></c:button>
                </td>
            </tr>
            <tr id="rowItemTypes">
                <td valign="top" id="itemTypes">

                    <div class="tab-body active">
                        <progress id="progBar"></progress>
                    </div>

                </td> 
            </tr>
            <tr class="autoheight"></tr>    
            <tr id="FooterRow">
                <td class="footer" align="right" colspan="3">
                    <div class="BtnWrapper">
                        <c:button id="BtnOk" runat="server" tabindex="2" label="OK" disabled="true"></c:button>
                        <c:button id="BtnCancel" runat="server" tabindex="3" label="Cancel"></c:button>
                    </div>
                </td>
            </tr>
        </table>
    </body>
</html>