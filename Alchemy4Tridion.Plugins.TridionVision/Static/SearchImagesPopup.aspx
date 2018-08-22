<html id="DocumentOptionsPopup" class="apopup" xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Auto-classify Images</title>
        <link rel='shortcut icon' type='image/x-icon' href='${ImgUrl}favicon.png' />
    </head>
    <body id="StackElement" style="height: 580px;">
        <table border="0" style="table-layout: fixed; width:90%; height:90%;">
            <tr id="rowHeader">
                <td colspan="2">
                    <span id="report-options-header">Auto-classify Images</span>
                </td>
            </tr>
            <tr id="rowLabel">
                <td>
                    <div style="display: inline-block; vertical-align: middle !important; padding-left: 10px; height: 21px !important;">
                        <c:button id="BtnGenerate" runat="server" tabindex="1" label="Generate Keywords"></c:button>
                    </div>
                </td>
                <td>
                    <div style="text-align: right;">
                        <div style="display: inline-block; vertical-align: middle !important; padding-left: 10px;">
                            Filter by:
                        </div>
                        <input id="word" style="display: inline-block; vertical-align: middle !important" placeholder="all" />
                        <div style="display: inline-block; vertical-align: middle !important; height: 21px !important;">
                            <c:button id="BtnSearch" runat="server" tabindex="1" label="Go"></c:button>
                        </div>
                    </div>
                </td>
            </tr>
            <tr id="rowTable" colspan="2">
                <td id="itemTypes">

                    <div class="tab-body active">
                        
                    </div>

                </td> 
            </tr>
            <tr id="FooterRow" colspan="2">
                <td>
                    <div style="padding-left: 10px;">
                        <c:button id="BtnOk" runat="server" tabindex="2" label="Close"></c:button>
                    </div>
                </td>
            </tr>
        </table>
    </body>
</html>