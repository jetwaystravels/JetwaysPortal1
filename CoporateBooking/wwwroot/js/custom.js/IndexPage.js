﻿//**********Validation home page  Code Start ****************//
//*****************************************************//
localStorage.clear();



function validateForm() {
    $('#multipleValuesForm').submit(function (event) {
        var legalEntity = document.getElementById("legalEntitySelect").value;
        var employee = document.getElementById("employeeSelect").value;

        // Check if Legal Entity and Employee are selected
        if (legalEntity === "") {
            alert("Please select a company.");
            return false; // Prevent form submission
        }

        if (employee === "") {
            alert("Please select an employee.");
            return false; // Prevent form submission
        }

        // If both fields are valid, the form will be submitted


        var originfocus = document.getElementById("myInput").value;
        if (originfocus == "") {
            //alert("Enter Origin Name");
            var originToDisplay = document.getElementById("errororigin");
            originToDisplay.style.display = "block";
            document.getElementById("myInput").focus();
            return false;
            event.preventDefault(); // Stop the form submission
        }
        var destinationfocus = document.getElementById("myInput1").value;
        if (destinationfocus == "") {
            //alert("Enter Origin Name");
            var originToDisplay = document.getElementById("errordestinaton");
            originToDisplay.style.display = "block";
            document.getElementById("myInput1").focus();
            return false;
        }

        if (originfocus == destinationfocus) {
            alert("Departure and Arrival Airport are same please change it..")
            return false;
            event.preventDefault(); // Stop the form submission

        }

        var startdate = document.getElementById("start-date").value;
        if (startdate == "") {
            //alert("Enter Origin Name");
            var starterror = document.getElementById("startDate");
            starterror.style.display = "block";
            document.getElementById("start-date").focus();
            return false;
            event.preventDefault(); // Stop the form submission
        }

        if ($('#round-tripid').is(':checked')) {
            var endtdate = document.getElementById("end-date").value;
            if (endtdate == "") {
                //alert("Enter Origin Name");
                var enderror = document.getElementById("endDate");
                enderror.style.display = "block";
                document.getElementById("end-date").focus();
                return false;
                event.preventDefault(); // Stop the form submission
            }
        }

        // Traveller Count Validation 9
        var adultCount = parseInt(document.getElementById('field_adult').value);
        var childCount = parseInt(document.getElementById('field_child').value);
        //var infantCount = parseInt(document.getElementById('field_infant').value);
        var totalCount = adultCount + childCount;
        if (totalCount > 9) {
            alert("The combined total of adults and children must be 9 or less.");
            return false;
            event.preventDefault();
        }

        //////*****************/////////
        var loader = document.getElementById("loader");
        loader.style.display = "block";
        $("body").css("overflow", "hidden");
        var navtopright = document.getElementById("top-navbar-nav");
        navtopright.style.display = "none";
        var logotop = document.querySelector(".navbar-brand");
        logotop.style.display = "none";
    });

    //var selectedClass = document.querySelector('input[name="flightclass"]:checked').value;
    //alert("1"+selectedClass);
    //localStorage.setItem('selectedFlightClass', selectedClass);
    //var selectedClass = localStorage.getItem('selectedFlightClass');
    //alert("2"+selectedClass);
    //if (selectedClass) {
    //    document.querySelector('.flightClassName').textContent = selectedClass;
    //}
};

function checkOnlyThis(checkbox) {
    var checkboxes = document.getElementsByName('farecheck');
    checkboxes.forEach((item) => {
        if (item !== checkbox) {
            item.checked = false;
        }
    });
}

//**********Chosen Arrival Code Start ****************//
//*****************************************************//
$(document).ready(function () {
    $('#arrivalItemId').chosen();
    $('#arrivalItemId').trigger('chosen:open');
    $('#basic-addon1').on('click', function (event) {
        event.stopPropagation();
        $('#arrivalItemId').trigger('chosen:open');
        $('.chosen-drop').show();
        $('.autoarrival input').attr('placeholder', 'To');
        $('.autoarrival').show();
        $('.autodropdown').hide();
        var chosenInput2 = document.querySelector('.autoarrival input');
        chosenInput2.focus();
        //chosenInput2.addEventListener("keydown", function (event) {
        //    if (event.key === " ") {
        //        event.preventDefault();
        //        alert("Whitespace is not allowed");
        //    }
        //});

        if (!chosenInput2.hasAttribute("onkeyup")) {
            chosenInput2.setAttribute("onkeyup", "filterCities(this.value)");
        }
        chosenInput2.addEventListener("paste", function (event) {
            event.preventDefault();
        });

    });

    $(document.body).on('click', function (event) {
        if (!$(event.target).closest('.chosen-drop').length && !$(event.target).is('#myInput1')) {
            $('.chosen-drop').hide();
        }
    });


});



document.addEventListener('DOMContentLoaded', function () {


    var input = document.getElementById("myInput1");
    input.addEventListener("keydown", function (event) {
        event.preventDefault();
    });

    // Disable pasting
    input.addEventListener("paste", function (event) {
        event.preventDefault();
    });
    var inputdate1 = document.getElementById("start-date");
    inputdate1.addEventListener("keydown", function (event) {
        event.preventDefault();
    });

    // Disable pasting
    inputdate1.addEventListener("paste", function (event) {
        event.preventDefault();
    });
    var inputdate2 = document.getElementById("end-date");
    inputdate2.addEventListener("keydown", function (event) {
        event.preventDefault();
    });

    // Disable pasting
    inputdate2.addEventListener("paste", function (event) {
        event.preventDefault();
    });

    $('.chosen-drop').css('display', 'none');
    var CityName = "Mumbai";
    var CityCode = "BOM";
    var AirportName = "Chhatrapati Shivaji Maharaj International Airport";
    document.getElementById("myInput1").value = CityName + "-" + CityCode;
    document.getElementById("airportArval").innerHTML = AirportName;
    var airportElement = document.getElementById("airportArval");
    airportElement.innerHTML = AirportName;
    airportElement.title = AirportName;

});

function toggleDropdownArrival() {
    //$('.chosen-drop').toggle();
    var chosenInputA = document.querySelector('.autoarrival input');
    chosenInputA.focus();
    if ($('.chosen-drop').is(':visible')) {
        $('.autoarrival input').focus();
        $('.chosen-drop').show();
    }
}


function arrivalSelection() {
    // Debugging (optional during dev)
    debugger;

    // Get dropdown and selected option
    var dropdown = document.getElementById("arrivalItemId");
    var selectedOption = dropdown.options[dropdown.selectedIndex];

    // Defensive check
    if (!selectedOption || !selectedOption.text) return;

    // Extract city and airport info
    var cityDetails = selectedOption.text.split('-');
    var cityName = cityDetails[0]?.trim() || "";
    var cityCode = cityDetails[1]?.trim() || "";
    var airportName = cityDetails[2]?.trim() || "";

    // Update input and label
    document.getElementById("myInput1").value = `${cityName} - ${cityCode}`;
    var airportElement = document.getElementById("airportArval");
    airportElement.innerHTML = airportName;
    airportElement.title = airportName;

    // Hide any custom dropdowns (optional UI logic)
    $('.chosen-drop').hide();

    // Reset dropdown selection
    dropdown.selectedIndex = -1;

    // ✅ Auto-focus and open Departure Date calendar
    var departureInput = document.getElementById("start-date");
    if (departureInput) {
        departureInput.focus(); // Focus ensures calendar hint
        if (typeof departureInput.showPicker === "function") {
            departureInput.showPicker(); // Chrome/Edge only
        }
    }
}




//**********Chosen Departure Code Start ****************//
//*****************************************************//
$(document).ready(function () {

    $('#selectedItemId').chosen();
    $('.chosen-drop').hide();
    $('#selectedItemId').trigger('chosen:open');

    $('#myInputbx').on('click', function (event) {

        event.stopPropagation();
        $('#selectedItemId').trigger('chosen:open');
        $('.chosen-drop').show();
        $('.chosen-search input').attr('placeholder', ' From');
        $('.autoarrival').hide();
        $('.autodropdown').show();
        var chosenInput = document.querySelector('.chosen-search input');
        chosenInput.focus();
        //chosenInput.addEventListener("keydown", function (event) {
        //    if (event.key === " ") {    
        //        event.preventDefault(); 
        //        alert("Whitespace is not allowed");
        //    }
        //});

        if (!chosenInput.hasAttribute("onkeyup")) {
            chosenInput.setAttribute("onkeyup", "filterCities(this.value)");
        }
        chosenInput.addEventListener("paste", function (event) {
            event.preventDefault();
        });
    });


    $(document.body).on('click', function (event) {
        if (!$(event.target).closest('.chosen-drop').length && !$(event.target).is('#myInput')) {
            $('.chosen-drop').hide();
        }
    });
});



let currentResults = [];

function filterCities(query) {
    if (query.length < 3) return;
    fetch(`/FlightSearchIndex/GetFilteredCities?query=${encodeURIComponent(query)}`)
        .then(response => response.json())
        .then(data => {
            currentResults = data;
            updateDropdown(currentResults, query);
        });
}


function updateDropdown(data, query) {
    const isArrivalDropdownVisible = $('#arrivalItemId').next('.chosen-container').find('.chosen-drop').is(':visible');
    const dropdown = document.getElementById(isArrivalDropdownVisible ? "arrivalItemId" : "selectedItemId");
    const inputField = document.querySelector(isArrivalDropdownVisible ? '.autoarrival input' : '.chosen-search input');
    const currentInput = inputField.value;

    dropdown.innerHTML = `<option value="" style="display: none;">Please select</option>
        ${data.map(item => `<option value="${item.citycode}">${item.cityname} - ${item.citycode} - ${item.airportname}</option>`).join('')}`;

    $(dropdown).trigger("chosen:updated");

    if (currentInput === query) {
        inputField.value = currentInput;
    }
    inputField.focus();
}


$(document).ready(function () {
    const inputBox = document.getElementById("myInput");
    inputBox.addEventListener("input", function () {
        const currentValue = inputBox.value;
    });
});



document.addEventListener('DOMContentLoaded', function () {
    $('.chosen-drop').css('display', 'none');
    var defaultCityName = "New Delhi";
    var defaultCityCode = "DEL";
    var defaultAirportName = "Indira Gandhi International Airport";
    document.getElementById("myInput").value = defaultCityName + "-" + defaultCityCode;
    document.getElementById("airportName").innerHTML = defaultAirportName;
    var airportElement = document.getElementById("airportName");
    airportElement.innerHTML = defaultAirportName;
    airportElement.title = defaultAirportName;

    var inputdep = document.getElementById("myInput");
    inputdep.addEventListener("keydown", function (event) {
        event.preventDefault();
    });

    // Disable pasting
    inputdep.addEventListener("paste", function (event) {
        event.preventDefault();
    });

});


function toggleDropdown() {
    //$('.chosen-drop').toggle();
    var chosenInput = document.querySelector('.chosen-search input');
    chosenInput.focus();
    if ($('.chosen-drop').is(':visible')) {
        $('.chosen-search input').focus();
        $('.chosen-drop').show();
    }
}

function handleSelection() {
    $('.chosen-search input').focus();
    //alert("Hello Test");
    var dropdown = document.getElementById("selectedItemId");
    var selectedValue = dropdown.value;
    var selectedOption = dropdown.options[dropdown.selectedIndex];
    var cityDetails = selectedOption.text.split('-');
    var cityName = cityDetails[0].trim();
    var cityCode = cityDetails[1].trim();
    var airportName = cityDetails[2].trim();


    //alert("Selected City Name: " + cityName);
    //alert("Selected City Code: " + cityCode);
    //alert("Selected Airport Name: " + airportName);
    document.getElementById("myInput").value = cityName + "-" + cityCode;
    document.getElementById("airportName").innerHTML = airportName;
    var airportElement = document.getElementById("airportName");
    airportElement.innerHTML = airportName;
    airportElement.title = airportName;
    $('.chosen-drop').hide();
    dropdown.selectedIndex = -1;
}
//***********From Cityname DropDown end********************//






////*****-----------RoundTrip end date disabled--********------//
$(document).ready(function () {



    //****  Replace Icon JS Start*/
    $('.rplc-btn').on('click', function (event) {
        var flyingFromValue = $('#myInput').val();
        var flyingToValue = $('#myInput1').val();
        //var airportElement = document.getElementById("airportName");
        //var airportName = airportElement.textContent;
        //var airportElement2 = document.getElementById("airportArval");
        //var airportName2 = airportElement2.textContent;

        // Swap input field values
        $('#myInput').val(flyingToValue);
        $('#myInput1').val(flyingFromValue);

        // Swap text content of airport elements
        var airportName = $('#airportName').text();
        var airportName2 = $('#airportArval').text();
        $('#airportName').text(airportName2).attr('title', airportName2);
        $('#airportArval').text(airportName).attr('title', airportName);



        // Check if departure and arrival airports are the same
        if (flyingFromValue == flyingToValue || airportName == airportName2) {
            alert("Departure and Arrival Airport are the same. Please change it.");
            event.preventDefault();
            return false;

        }
    });

    //**********Replace Icon JS End**********/
    $('.S-option input[type="radio"]').on('change', function () {
        // Uncheck all radio buttons in the same group except the currently selected one
        $('.S-option input[type="radio"]').not(this).prop('checked', false);

    });

    //**********search old code start**********/

    //$('#end-date').prop('disabled', true);
    //$('#bgEnddate').css('background-color', '#e9ecef');


    //$('.endselect').on('click', function () {
    //    $('#round-tripid').prop('checked', true);
    //    let nextTwoDays = new Date();
    //    nextTwoDays.setDate(nextTwoDays.getDate() + 2);
    //    let formattedDate = nextTwoDays.toISOString().split('T')[0];
    //    $('#end-date').val(formattedDate);
    //    let nextOneDays = new Date();
    //    nextOneDays.setDate(nextOneDays.getDate() + 1);
    //    let OnewayDate = nextOneDays.toISOString().split('T')[0];
    //    $('#start-date').val(OnewayDate);
    //    $('#bgEnddate').css('background-color', '#fff');
    //    $('.hasDatepicker').css('background-color', '#fff');
    //    $('#end-date').css('visibility', 'visible');
    //    $('.hasDatepicker').prop('disabled', false);
    //    $('.rounddateinput').css('display', 'none');

    //});


    //// Event handler for radio button change
    //$('input[type="radio"]').on('change', function () {
    //    if ($('#round-tripid').is(':checked')) {

    //        $('#end-date').prop('disabled', false);
    //        $('#bgEnddate').css('background-color', '#fff');
    //        $('#end-date').css('visibility', 'visible');
    //        //Date Picker end date


    //        var returndate = new Date();
    //        returndate.setDate(returndate.getDate() + 2); // Add 2 days to the current date
    //        debugger;
    //        var returndd = returndate.getDate(); // Note: Don't pad with 0
    //        var returnmm = returndate.getMonth() + 1; // Note: Don't pad with 0
    //        var returnyyyy = returndate.getFullYear();

    //        // Check if the return day exceeds the maximum number of days in the month
    //        if (returndd > new Date(returnyyyy, returnmm, 0).getDate()) {
    //            returndd = new Date(returnyyyy, returnmm, 0).getDate(); // Set to the last day of the month
    //        }

    //        // Format the date to yyyy-mm-dd, ensuring each part is padded with a leading zero if necessary
    //        var returncurrentDate = returnyyyy + '-' + (returnmm < 10 ? '0' + returnmm : returnmm) + '-' + (returndd < 10 ? '0' + returndd : returndd);

    //        var startdate = new Date();
    //        startdate.setDate(startdate.getDate() + 1);
    //        var startdd = startdate.getDate(); // Note: Don't pad with 0
    //        var startnmm = startdate.getMonth() + 1; // Note: Don't pad with 0
    //        var startyyyy = startdate.getFullYear();

    //        // Check if the return day exceeds the maximum number of days in the month
    //        if (startdd > new Date(startyyyy, startnmm, 0).getDate()) {
    //            startdd = new Date(startyyyy, startnmm, 0).getDate(); // Set to the last day of the month
    //        }

    //        // Format the date to yyyy-mm-dd, ensuring each part is padded with a leading zero if necessary
    //        var startcurrentDate = startyyyy + '-' + (startnmm < 10 ? '0' + startnmm : startnmm) + '-' + (startdd < 10 ? '0' + startdd : startdd);

    //        $("#end-date").val(returncurrentDate);
    //        //$("#end-date").datepicker({
    //        //    dateFormat: 'yy-mm-dd',
    //        //    numberOfMonths: 2,
    //        //    maxDate: '+6m',
    //        //    minDate: '0',
    //        //    onSelect: function (selectedDate) {
    //        //        var endDate = $(this).datepicker('getDate');
    //        //        $("#start-date").datepicker("option", "maxDate", endDate);
    //        //    }
    //        //});

    //        $("#start-date").val(startcurrentDate);
    //        //$("#start-date").datepicker({
    //        //    dateFormat: 'yy-mm-dd',
    //        //    numberOfMonths: 2,
    //        //    maxDate: '+6m',
    //        //    minDate: '0',
    //        //    onSelect: function (selectedDate) {
    //        //        var startDate = $(this).datepicker('getDate');
    //        //        $("#end-date").datepicker("option", "minDate", startDate);
    //        //    }
    //        //});

    //        const elementToHide = document.querySelector('.rounddateinput');
    //        elementToHide.style.display = 'none'; // Hide the element


    //        ///Date picker End Date---End--
    //    }

    //    else if ($('#multi-cityid').is(':checked')) {

    //        $('#end-date').prop('disabled', false);
    //        $('#bgEnddate').css('background-color', '#fff');
    //        $('#end-date').css('visibility', 'visible');
    //        //Date Picker end date



    //        var returndate = new Date();
    //        returndate.setDate(returndate.getDate() + 2); // Add 2 days to the current date
    //        debugger;
    //        var returndd = returndate.getDate(); // Note: Don't pad with 0
    //        var returnmm = returndate.getMonth() + 1; // Note: Don't pad with 0
    //        var returnyyyy = returndate.getFullYear();

    //        // Check if the return day exceeds the maximum number of days in the month
    //        if (returndd > new Date(returnyyyy, returnmm, 0).getDate()) {
    //            returndd = new Date(returnyyyy, returnmm, 0).getDate(); // Set to the last day of the month
    //        }

    //        // Format the date to yyyy-mm-dd, ensuring each part is padded with a leading zero if necessary
    //        var returncurrentDate = returnyyyy + '-' + (returnmm < 10 ? '0' + returnmm : returnmm) + '-' + (returndd < 10 ? '0' + returndd : returndd);

    //        var startdate = new Date();
    //        startdate.setDate(startdate.getDate() + 1);
    //        var startdd = startdate.getDate(); // Note: Don't pad with 0
    //        var startnmm = startdate.getMonth() + 1; // Note: Don't pad with 0
    //        var startyyyy = startdate.getFullYear();

    //        // Check if the return day exceeds the maximum number of days in the month
    //        if (startdd > new Date(startyyyy, startnmm, 0).getDate()) {
    //            startdd = new Date(startyyyy, startnmm, 0).getDate(); // Set to the last day of the month
    //        }

    //        // Format the date to yyyy-mm-dd, ensuring each part is padded with a leading zero if necessary
    //        var startcurrentDate = startyyyy + '-' + (startnmm < 10 ? '0' + startnmm : startnmm) + '-' + (startdd < 10 ? '0' + startdd : startdd);

    //        $("#end-date").val(returncurrentDate);

    //        //$("#end-date").datepicker({
    //        //    dateFormat: 'yy-mm-dd',
    //        //    numberOfMonths: 2,
    //        //    maxDate: '+3m',
    //        //    minDate: '0'
    //        //});

    //        $("#start-date").val(startcurrentDate);
    //        //$("#start-date").datepicker(
    //        //    {
    //        //        dateFormat: 'yy-mm-dd',
    //        //        numberOfMonths: 2,
    //        //        maxDate: '+2m',
    //        //        minDate: '0',
    //        //        onSelect: function (selectedDate) {
    //        //            var endDate = $('#end-date');
    //        //            endDate.datepicker('option', 'minDate', selectedDate);
    //        //            endDate.datepicker('setDate', selectedDate);

    //        //        }
    //        //    });

    //        const elementToHide = document.querySelector('.rounddateinput');
    //        elementToHide.style.display = 'none'; // Hide the element


    //        ///Date picker End Date---End--
    //    }




    //    else {
    //        // Disable the end date input field for other options
    //        $('#end-date').prop('disabled', true);
    //        $('#bgEnddate').css('background-color', '#e9ecef');
    //        $('#end-date').css('visibility', 'hidden');
    //        const elementToHide = document.querySelector('.rounddateinput');
    //        elementToHide.style.display = 'block'; // Hide the element

    //    }

    //});


    //$(function () {
    //    var today = new Date();
    //    var dd = String(today.getDate()).padStart(2, '0');
    //    var mm = String(today.getMonth() + 1).padStart(2, '0'); // January is 0!
    //    var yyyy = today.getFullYear();
    //    var maxDays = new Date(yyyy, mm, 0).getDate(); // Get the maximum days for the current month
    //    var currentDate = yyyy + '-' + mm + '-' + dd;

    //    // Set the current date as the default value in the input field
    //    $("#start-date").val(currentDate);
    //    //$("#start-date").datepicker({
    //    //    dateFormat: 'yy-mm-dd',
    //    //    numberOfMonths: 2,
    //    //    maxDate: '+' + maxDays + 'd', // Set the maximum date to the last day of the current month
    //    //    minDate: '0',
    //    //    onSelect: function (selectedDate) {
    //    //        var endDate = $('#end-date');
    //    //        endDate.datepicker('option', 'minDate', selectedDate);
    //    //        endDate.datepicker('setDate', selectedDate);
    //    //    }
    //    //});
    //});

    //$(function () {
    //    $("#start-date").datepicker({
    //        dateFormat: 'yy-mm-dd',
    //        numberOfMonths: 2,
    //        maxDate: '+12m',
    //        minDate: '0',
    //        onSelect: function (selectedDate) {
    //            var startDate = $(this).datepicker('getDate');
    //            $("#end-date").datepicker("option", "minDate", startDate);
    //        }
    //    });

    //    $("#end-date").datepicker({
    //        dateFormat: 'yy-mm-dd',
    //        numberOfMonths: 2,
    //        maxDate: '+12m',
    //        minDate: '0',
    //        onSelect: function (selectedDate) {
    //            var endDate = $(this).datepicker('getDate');
    //            $("#start-date").datepicker("option", "maxDate", endDate);
    //        }
    //    });
    //});

    //**********search old code End**********/


    //**********search New code Start**********/

    // Global flag to track if user selected return date manually
    let returnDateWasManuallySelected = false;

    // Initially disable return date input
    $('#end-date').prop('disabled', true);
    $('#bgEnddate').css('background-color', '#e9ecef');

    // ✅ Modified: Handle endselect click (first time only)
    $('.endselect').on('click', function () {
        $('#round-tripid').prop('checked', true);

        // If user already selected a return date manually, do not auto-change
        if (returnDateWasManuallySelected) return;

        // Get start date and add 1 day for default return date
        let depDateStr = $('#start-date').val();
        let depDate = new Date(depDateStr);
        depDate.setDate(depDate.getDate() + 1);
        let returnDateStr = depDate.toISOString().split('T')[0];

        $('#end-date').val(returnDateStr);
        $('#end-date').prop('disabled', false);
        $('#end-date').css('visibility', 'visible');
        $('#bgEnddate').css('background-color', '#fff');
        $('.hasDatepicker').css('background-color', '#fff');
        $('.rounddateinput').css('display', 'none');
    });

    // ✅ Radio button change logic
    $('input[type="radio"]').on('change', function () {
        returnDateWasManuallySelected = false;

        if ($('#round-tripid').is(':checked') || $('#multi-cityid').is(':checked')) {
            $('#end-date').prop('disabled', false);
            $('#bgEnddate').css('background-color', '#fff');
            $('#end-date').css('visibility', 'visible');

            let today = new Date();
            let returndate = new Date();
            returndate.setDate(today.getDate() + 2);
            let startdate = new Date();
            startdate.setDate(today.getDate() + 1);

            let returncurrentDate = returndate.toISOString().split('T')[0];
            let startcurrentDate = startdate.toISOString().split('T')[0];

            $("#end-date").val(returncurrentDate);
            $("#start-date").val(startcurrentDate);

            const elementToHide = document.querySelector('.rounddateinput');
            elementToHide.style.display = 'none';
        } else {
            $('#end-date').prop('disabled', true);
            $('#bgEnddate').css('background-color', '#e9ecef');
            $('#end-date').css('visibility', 'hidden');

            const elementToHide = document.querySelector('.rounddateinput');
            elementToHide.style.display = 'block';
        }
    });

    // ✅ Set default today in start-date
    $(function () {
        let today = new Date();
        let currentDate = today.toISOString().split('T')[0];
        $("#start-date").val(currentDate);
    });

    // ✅ DatePicker setup
    $(function () {
        // Departure Date
        $("#start-date").datepicker({
            dateFormat: 'yy-mm-dd',
            numberOfMonths: 2,
            maxDate: '+12m',
            minDate: '0',
            onSelect: function (selectedDate) {
                let startDate = $(this).datepicker('getDate');
                $("#end-date").datepicker("option", "minDate", startDate);

                // ✅ Reset return date selection flag if departure changes
                returnDateWasManuallySelected = false;
            }
        });

        // Return Date
        $("#end-date").datepicker({
            dateFormat: 'yy-mm-dd',
            numberOfMonths: 2,
            maxDate: '+12m',
            minDate: '0',
            onSelect: function (selectedDate) {
                let endDate = $(this).datepicker('getDate');
                $("#start-date").datepicker("option", "maxDate", endDate);

                // ✅ Set flag that user manually selected return date
                returnDateWasManuallySelected = true;
            }
        });
    });

    //**********search New code End**********/



    var maxField = 6;
    var maxField_adult = 9;
    var count_adult = parseInt(localStorage.getItem("adultcount")) || 1; // Retrieve adult count from local storage or default to 1
    var count_child = 0;
    var count_infant = 0;

    $('#count_adult').text(count_adult);
    $('#field_adult').val(count_adult);

    $('.increment').on('click', function () {
        if (count_adult < maxField_adult) {
            count_adult++;
            $('#count_adult').text(count_adult);
            $('#field_adult').val(count_adult);
            localStorage.setItem("adultcount", count_adult); // Store adult count in local storage
        }
    });

    $('.decrement').on('click', function () {
        if (count_adult > 1) {
            count_adult--;
            $('#count_adult').text(count_adult);
            $('#field_adult').val(count_adult);
            localStorage.setItem("adultcount", count_adult); // Store adult count in local storage

            // Adjust infant count to match adult count
            if (count_infant > count_adult) {
                count_infant = count_adult;
                $('#count_infant').text(count_infant);
                $('#field_infant').val(count_infant);
            }
        }
    });

    $('.increment1').on('click', function () {
        if (count_child < maxField) {
            count_child++;
            $('#count_child').text(count_child);
            $('#field_child').val(count_child);
        }
    });

    $('.decrement1').on('click', function () {
        if (count_child > 0) {
            count_child--;
            $('#count_child').text(count_child);
            $('#field_child').val(count_child);
        }
    });

    $('.increment2').on('click', function () {
        if (count_infant < maxField) {
            var adultcounttotal = parseInt(localStorage.getItem("adultcount")) || 1; // Retrieve adult count from local storage or default to 1
            if (count_infant >= adultcounttotal) {
                alert("Number of infants cannot be more than adults");
                return false;
            }
            count_infant++;
            $('#count_infant').text(count_infant);
            $('#field_infant').val(count_infant);
        }
    });

    $('.decrement2').on('click', function () {
        if (count_infant > 0) {
            count_infant--;
            $('#count_infant').text(count_infant);
            $('#field_infant').val(count_infant);
        }
    });

});






