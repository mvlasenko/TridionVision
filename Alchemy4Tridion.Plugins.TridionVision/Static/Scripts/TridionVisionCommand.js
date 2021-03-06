﻿/**
 * Creates an anguilla command using a wrapper shorthand.
 *
 * Note the ${PluginName} will get replaced by the actual plugin name.
 */
Alchemy.command("${PluginName}", "TridionVisionCommand", {

    /**
     * If an init function is created, this will be called from the command's constructor when a command instance
     * is created.
     */
    init: function () {
        console.log("Init Tridion Vision Command");
    },

    /**
     * Whether or not the command is enabled for the user (will usually have extensions displayed but disabled).
     * @returns {boolean}
     */
    isEnabled: function (selection) {

        var count = selection.getCount();
        if (!count)
            return false;

        for (var i = 0; i < count; i++) {

            var itemUri = selection.getItem(i);
            if (!itemUri)
                continue;

            var itemType = $models.getItemType(itemUri);

            if (
                itemType === $const.ItemType.FOLDER || itemType === $const.ItemType.COMPONENT
            ) { }
            else {
                return false;
            }
        }

        if (!window.currentFolderImagesCounts)
            return true;

        for (var i = 0; i < window.currentFolderImagesCounts.length; i++) {
            if (window.currentFolderImagesCounts[i].split('-')[1] == itemUri.split('-')[1])
                return true;
        }

        return false;
    },


    /**
     * Whether or not the command is available to the user.
     * @returns {boolean}
     */
    isAvailable: function (selection) {

        var count = selection.getCount();
        if (!count)
            return false;

        for (var i = 0; i < count; i++) {

            var itemUri = selection.getItem(i);
            if (!itemUri)
                continue;

            var itemType = $models.getItemType(itemUri);

            if (
                itemType === $const.ItemType.FOLDER || itemType === $const.ItemType.COMPONENT
            ) { }
            else {
                return false;
            }
        }

        if (!window.currentFolderImagesCounts)
            return true;

        for (var i = 0; i < window.currentFolderImagesCounts.length; i++) {
            if (window.currentFolderImagesCounts[i].split('-')[1] == itemUri.split('-')[1])
                return true;
        }

        return false;
    },

    execute: function (selection) {

        if (window.currentFolderImagesCount > 10)
        {
            alert("This folder contains more than 10 multimedia components. Please select another folder.");
            return false;
        }

        var uri = "";

        // Gets the items ids
        var count = selection.getCount();
        for (var i = 0; i < count; i++) {

            var itemUri = selection.getItem(i);
            uri += itemUri.replace("tcm:", "");

            if (i < count - 1) {
                uri += "|";
            }
        }

        var url = "${ViewsUrl}SearchImagesPopup.aspx#uri=" + uri;
        var args = { popupType: Tridion.Controls.Popup.Type.MODAL_IFRAME, uri: uri };
        var features = { height: 600, width: 800 };

        var popup = $popup.create(url, features, args);

        $evt.addEventHandler(popup, "cancel", function (e) {
            $evt.removeAllEventHandlers(popup, "cancel");
            popup.close();
            popup.dispose();
            popup = null;
        });

        popup.open();

        window.SearchImagesPopup = popup;
    }

});