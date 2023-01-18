function validateInputs() {
    let validationOK = false;

    if ($("#trackingNumber").val() === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Oops...',
            text: 'The tracking number is required.'
        });
    }
    else if ($("#carrier").val() === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Oops...',
            text: 'The carrier is required.'
        });
    }
    else {
        validationOK = true;
    }

    return validationOK;
}
