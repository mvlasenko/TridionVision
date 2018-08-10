Type.registerNamespace("Alchemy4Tridion.Plugins.TridionVision");

/*
* Constructor
*/
Alchemy4Tridion.Plugins.TridionVision.SearchImagesPopup = function (element) {
    Type.enableInterface(this, "Alchemy4Tridion.Plugins.TridionVision.SearchImagesPopup");
    this.addInterface("Tridion.Controls.ModalPopupView");
    this.addInterface("Tridion.Cme.View");
};

/*
* Initialize the pop up window
*/
Alchemy4Tridion.Plugins.TridionVision.SearchImagesPopup.prototype.initialize = function () {

    $j = Alchemy.library("jQuery");

    console.log("Initializing Search Images Popup...");

    this.callBase("Tridion.Cme.View", "initialize");

    var p = this.properties;
    var c = p.controls;

    var uri = $url.getHashParam("uri");

    c.BtnSearch = $controls.getControl($("#BtnSearch"), "Tridion.Controls.Button");
    c.BtnOk = $controls.getControl($("#BtnOk"), "Tridion.Controls.Button");
    c.BtnCancel = $controls.getControl($("#BtnCancel"), "Tridion.Controls.Button");

    $evt.addEventHandler(c.BtnSearch, "click", function (e) {
        showItems(uri, $j("#word").val());
    });

    $evt.addEventHandler(c.BtnOk, "click", function (e) {
        var reportOptionsPopup = window.parent.SearchImagesPopup;
        reportOptionsPopup.close();
    });

    $evt.addEventHandler(c.BtnCancel, "click", function (e) {
        var reportOptionsPopup = window.parent.SearchImagesPopup;
        reportOptionsPopup.close();
    });

};

function showItems(tcmFolder, word) {

    //todo: disable buttons

    //enable progress bar
    $j("#progBar").show();

    // This is the call to my controller where the core service code is used get the list of items
    Alchemy.Plugins["${PluginName}"].Api.TridionVisionService.getItems(tcmFolder, word).success(function (items) {

        //disable progress bar
        $j("#progBar").hide();

        //show list of items
        $j(".tab-body.active").empty();
        $j(".tab-body.active").append(items);

        //todo: change buttons visibility
    })
    .error(function (error) {
        console.log("There was an error", error);
        $messages.registerError("An error has occurred", error && error.exceptionMessage ? error.exceptionMessage : null, null, true, false);
    })
    .complete(function () {
        // this is called regardless of success or failure.
        //todo: enable buttons
    });
}

$display.registerView(Alchemy4Tridion.Plugins.TridionVision.SearchImagesPopup);