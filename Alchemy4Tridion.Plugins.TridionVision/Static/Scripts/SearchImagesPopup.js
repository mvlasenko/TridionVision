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

    $evt.addEventHandler(c.BtnSearch, "click", function (e) {
        var word = $j("#word").val();
        if(!word)
            word = "all";

        showItems(uri, word);
    });

    $evt.addEventHandler(c.BtnOk, "click", function (e) {
        var reportOptionsPopup = window.parent.SearchImagesPopup;
        reportOptionsPopup.close();
    });

};

function showItems(tcmFolder, word) {

    //enable progress bar
    $j(".tab-body.active").empty();
    $j(".tab-body.active").append("<progress></progress>");

    // This is the call to my controller where the core service code is used get the list of items
    Alchemy.Plugins["${PluginName}"].Api.TridionVisionService.getItems(tcmFolder, word).success(function (items) {

        //show list of items
        $j(".tab-body.active").empty();
        $j(".tab-body.active").append(items);

        setupForItemClicked();

    })
    .error(function (error) {
        console.log("There was an error", error);
        $messages.registerError("An error has occurred", error && error.exceptionMessage ? error.exceptionMessage : null, null, true, false);
    })
    .complete(function () {
        // this is called regardless of success or failure.
    });
}

function setupForItemClicked() {

    // We want to have an action when we click anywhere on the tab body
    // that isn't a used or using item
    $j(".tab-body").mouseup(function (e) {
        // To do this we first find the results item containing the not used items
        var results = $j(".results");
        if (!results.is(e.target) // if the target of the click isn't the results...
        && results.has(e.target).length === 0) // ... nor a descendant of the results
        {
            // deselect the current item
            $j(".item.selected").removeClass("selected");
        }
    });

    // An item is a Tridion item that is not being used by the current item (folder).
    // This is the click function for the items.
    $j(".item").click(function () {
        // When you click on an item we deselect any currently selected item
        $j(".item.selected").removeClass("selected");
        // And select the item you clicked on
        $j(this).addClass("selected");

        // Gets the selected item TCM
        var selectedItemId = $j(".item.selected").attr("id");

        //if id is set
        if (selectedItemId) {

            var url = "/WebUI/item.aspx?tcm=16#id=" + selectedItemId;
            var win = window.open(url, '_blank');
            win.focus();
        }
        else {
            // deselect the current item
            $j(".item.selected").removeClass("selected");
        }
    });
}

$display.registerView(Alchemy4Tridion.Plugins.TridionVision.SearchImagesPopup);