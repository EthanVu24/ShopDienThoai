function loadDataProductTable(name, page, minPrice, maxPrice, manufacturer, ram, storage) {
    $.ajax({
        type: "GET",
        url: "/Admin/ProductSearch",
        data: {
            name,
            page,
            minPrice,
            maxPrice,
            manufacturer,
            ram,
            storage
        },
        success: function (data) {
            $("#productTable").html(data);
        }
    })
}

function loadDataClientTable(name, status, page) {
    $.ajax({
        type: "GET",
        url: "/Admin/ClientSearch",
        data: {
            name,
            status,
            page
        },
        success: function (data) {
            $("#clientTable").html(data);
        }
    })
}

function loadDataAccountTable(name, status, page) {
    $.ajax({
        type: "GET",
        url: "/Admin/AccountSearch",
        data: {
            name,
            status,
            page
        },
        success: function (data) {
            $("#accountTable").html(data);
        }
    })
}

function loadDataManufacturerTable(name, page) {
    $.ajax({
        type: "GET",
        url: "/Admin/ManufacturerSearch",
        data: {
            name,
            page
        },
        success: function (data) {
            $("#manufacturerTable").html(data);
        }
    })
}

function loadDataSupplierTable(name, page) {
    $.ajax({
        type: "GET",
        url: "/Admin/SupplierSearch",
        data: {
            name,
            page
        },
        success: function (data) {
            $("#supplierTable").html(data);
        }
    })
}

$(document).ready(function () {
    $('.sub-item').on('click', function () {
        $(this).toggleClass("show");
    })

    $('#hambuger').on('click', function () {
        $('.sidebar').toggleClass("d-none");
        $('#content').toggleClass("extend");
    })

    loadDataProductTable("", 1, null, null, 0, null, null);
    loadDataClientTable("", null, 1);
    loadDataAccountTable("", null, 1);
    loadDataManufacturerTable("", 1);
    loadDataSupplierTable("", 1);

    //Product
    $("#formSearchProduct").on("submit", function (e) {
        e.preventDefault();
        var inputSearch = $("#inputSearchProduct").val();
        var inputMinPrice = $("#inputSearchProductMinPrice").val();
        var inputMaxPrice = $("#inputSearchProductMaxPrice").val();
        var inputManufacturer = $("#inputSearchProductManufacturer").val();
        var inputRam = $("#inputSearchProductRam").val();
        var inputStorage = $("#inputSearchProductStorage").val();
        loadDataProductTable(inputSearch, 1, inputMinPrice, inputMaxPrice, inputManufacturer, inputRam, inputStorage);
    })

    $(document).on('click', '.pagination .page-product a', function (e) {
        e.preventDefault();
        var page = $(this).attr("href").split("?page=")[1];
        var inputSearch = $("#inputSearchProduct").val();
        var inputMinPrice = $("#inputSearchProductMinPrice").val();
        var inputMaxPrice = $("#inputSearchProductMaxPrice").val();
        var inputManufacturer = $("#inputSearchProductManufacturer").val();
        var inputRam = $("#inputSearchProductRam").val();
        var inputStorage = $("#inputSearchProductStorage").val();
        loadDataProductTable(inputSearch, page, inputMinPrice, inputMaxPrice, inputManufacturer, inputRam, inputStorage);
    })

    //Client
    $("#formSearchClient").on("submit", function (e) {
        e.preventDefault();
        var inputSearch = $("#inputSearchClient").val();
        var inputStatus = $("#selectSearchClient").val();
        loadDataClientTable(inputSearch, inputStatus, 1);
    })

    $(document).on('click', '.pagination .page-client a', function (e) {
        e.preventDefault();
        var page = $(this).attr("href").split("?page=")[1];
        var inputSearch = $("#inputSearchClient").val();
        var inputStatus = $("#selectSearchClient").val();
        loadDataClientTable(inputSearch, inputStatus, page);
    })

    //Account
    $("#formSearchAccount").on("submit", function (e) {
        e.preventDefault();
        var inputSearch = $("#inputSearchAccount").val();
        var inputStatus = $("#selectStatusAccount").val();
        loadDataAccountTable(inputSearch, inputStatus, 1);
    })

    $(document).on('click', '.pagination .page-client a', function (e) {
        e.preventDefault();
        var page = $(this).attr("href").split("?page=")[1];
        var inputSearch = $("#inputSearchAccount").val();
        var inputStatus = $("#selectStatusAccount").val();
        loadDataAccountTable(inputSearch, inputStatus, page);
    })

    //Manufacturer
    $("#formSearchManufacturer").on("submit", function (e) {
        e.preventDefault();
        var inputSearch = $("#inputSearchManufacturer").val();
        loadDataManufacturerTable(inputSearch, 1);
    })

    $(document).on('click', '.pagination .page-manufacturer a', function (e) {
        e.preventDefault();
        var page = $(this).attr("href").split("?page=")[1];
        var inputSearch = $("#inputSearchManufacturer").val();
        loadDataManufacturerTable(inputSearch, page);
    })

    //Supplier
    $("#formSearchSupplier").on("submit", function (e) {
        e.preventDefault();
        var inputSearch = $("#inputSearchSupplier").val();
        loadDataSupplierTable(inputSearch, 1);
    })

    $(document).on('click', '.pagination .page-supplier a', function (e) {
        e.preventDefault();
        var page = $(this).attr("href").split("?page=")[1];
        var inputSearch = $("#inputSearchSupplier").val();
        loadDataSupplierTable(inputSearch, page);
    })
})