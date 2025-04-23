function initializeIngredientSearch(options) {
    const defaults = {
        searchInputId: null,
        dropdownid: null,
        url: 'Recipes/SearchIngredients',
        minSearchLength: 2,
        existingItemsCallback: function () { return []; },
        selectionCallback: function (selectedItem) {
            console.warn('Selection callback', selectedItem),
                clearInputOnSelect: true,
        preventEnterSubmit:true
    }
}