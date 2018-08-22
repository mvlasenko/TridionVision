!(function () {
    try {

        $evt.addEventHandler($display, "start", onDisplayStart);

        function onDisplayStart() {
            $evt.removeEventHandler($display, "start", onDisplayStart);

            var view = $display.getView();
            if (view && Tridion.OO.implementsInterface(view, "Tridion.Cme.Views.DashboardBase")) {
                var filteredDashboardList = $controls.getControl($("#FilteredDashboardList"), "Tridion.Controls.FilteredList");
                var list = filteredDashboardList && filteredDashboardList.getList();

                if (list) {
                    $evt.addEventHandler(list, "draw", saveFolderInfo);
                }
            }
        };

        function saveFolderInfo(event) {
            var locationId = $url.getHashParam("locationId");
            if (locationId) {
                //console.log(locationId);
                Alchemy.Plugins["${PluginName}"].Api.TridionVisionService.getCounts(locationId.replace("tcm:", "")).success(function (res) {
                    window.currentFolderImagesCounts = res;
                });

            }
        };
    }
    catch (error) {
        $messages.registerError("Alchemy Plugin Error: ", error);
    }

})();