function validatePrices() {
    let listPrice = $("#Product_ListPrice").val();
    let price = $("#Product_Price").val();
    let price50 = $("#Product_Price50").val();
    let price100 = $("#Product_Price100").val();

    let listPriceFloat = parseFloat(listPrice.replace(/\./g, '').replace(',', '.'));
    let priceFloat = parseFloat(price.replace(/\./g, '').replace(',', '.'));
    let price50Float = parseFloat(price50.replace(/\./g, '').replace(',', '.'));
    let price100Float = parseFloat(price100.replace(/\./g, '').replace(',', '.'));

    let validationOK = false;

    if (listPriceFloat <= 0 || listPriceFloat > 99999.99) {
        Swal.fire({
            icon: 'warning',
            title: 'Oops...',
            text: 'The list price must be between 0,01 and 99.999,99.'
        });
    }
    else if (priceFloat <= 0 || priceFloat > 99999.99) {
        Swal.fire({
            icon: 'warning',
            title: 'Oops...',
            text: 'The price must be between 0,01 and 99.999,99.'
        });
    }
    else if (price50Float <= 0 || price50Float > 99999.99) {
        Swal.fire({
            icon: 'warning',
            title: 'Oops...',
            text: 'The price 50 must be between 0,01 and 99.999,99.'
        });
    }
    else if (price100Float <= 0 || price100Float > 99999.99) {
        Swal.fire({
            icon: 'warning',
            title: 'Oops...',
            text: 'The price 100 must be between 0,01 and 99.999,99.'
        });
    }
    else {
        validationOK = true;
    }

    return validationOK;
}

function validateImage() {
    if ($("#Product_Image").val() === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Oops...',
            text: 'The image is required.'
        });

        return false;
    }

    return true;
}

function setPricesMask() {
    $("#Product_ListPrice").mask("00.000,00", { reverse: true, selectOnFocus: true });
    $("#Product_Price").mask("00.000,00", { reverse: true, selectOnFocus: true });
    $("#Product_Price50").mask("00.000,00", { reverse: true, selectOnFocus: true });
    $("#Product_Price100").mask("00.000,00", { reverse: true, selectOnFocus: true });

    $("#Product_ListPrice, #Product_Price, #Product_Price50, #Product_Price100").blur(function () {
        if ($(this).val() === "") {
            $(this).val("0,00");
        }
    });
}
