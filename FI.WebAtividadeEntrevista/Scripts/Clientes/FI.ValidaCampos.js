$(document).ready(function () {
    $("#CPF").inputmask("mask", { "mask": "999.999.999-99" }, { reverse: true });
    $("#CPFBeneficiario").inputmask("mask", { "mask": "999.999.999-99" }, { reverse: true });
})