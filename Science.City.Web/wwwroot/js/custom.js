
$(".imageValidate").change(function () {
	var fileExtension = ['jpeg', 'jpg', 'png', 'gif', 'bmp'];
	var targetElement = $(this).parents("td").next().find("img");
	if ($.inArray(($(this).context.files[0].name).split('.').pop().toLowerCase(), fileExtension) === -1) {
		$(this).parents("td").find(".field-validation-valid").text("Select valid image file.");
		$(targetElement).attr("src", "/img/staff_03.png");
		return;
	}
	$(this).parents("td").find(".field-validation-valid").text("");
	ImagePreview(targetElement, $(this).context);
})


//Deputy General Manager ->Tour Operator
$(".dgmT_tour_operator_Link").click(function () {

	var id = $(this).attr("data-id");
	var action = $(this).attr("data-action");
	var controller = $(this).attr("data-controller");
	var area = $(this).attr("data-area");
	var url = "/" + area + "/" + controller + "/" + action;
	var type = "GET";
	var datatype = "HTML";
	var data = { Id: id };
	var targetElement = $("#" + $(this).attr("href").replace("#", ""));
	var callback = function (response) {
		clearAllData();
		$(targetElement).empty();
		$(targetElement).append(response);
	}

	AJAXRequest(url, type, data, datatype, callback)
})

clearAllData = function () {
	var targetElement;
	$(".dgmT_tour_operator_Link").each(function () {
		targetElement = $("#" + $(this).attr("href").replace("#", ""));
		$(targetElement).empty();
	});
}

//Ticket Booking
$(".referalLetter").change(function () {
	$(this).parent('div').next().text($(this).context.files[0].name);
});
//MemberShip Profile
$(".profilePic").change(function () {
	debugger;
	var fileExtension = ['jpeg', 'jpg', 'png', 'gif', 'bmp'];
	var targetElement = $(this).parent(".cstm-choose-file-wrap").parent().next().find("img");
	if ($.inArray(($(this).context.files[0].name).split('.').pop().toLowerCase(), fileExtension) === -1) {
		debugger;
		$(this).parent(".cstm-choose-file-wrap").parent().find(".field-validation-valid").text("Select valid image file.");
		$(targetElement).attr("src", "/img/staff_03.png");
		return;
	}
	$(this).parent(".cstm-choose-file-wrap").parent().find(".field-validation-valid").text("");
	ImagePreview(targetElement, $(this).context);
})
//Function

ImagePreview = function (targetElement, element) {
	var fileData = element;
	if (fileData.files && fileData.files[0]) {
		var reader = new FileReader();
		reader.onload = function (e) {
			$(targetElement).attr("src", e.target.result);
		}
		reader.readAsDataURL(fileData.files[0]);
	}
}

//Role Changes
$("#RoleId").change(function () {
	roleChange();
});


roleChange = function () {
	debugger;
	if (thirdparty == $("#RoleId option:selected").text()) {
		$("#GSTNumber").parent(".form-group").show();
	}
	else {
		$("#GSTNumber").parent(".form-group").hide();
	}
}
roleChange();


roleChangeformedia = function () {
	debugger;
	if (Media == $("#RoleId option:selected").text()) {
		$("#PressReporterName").parent(".form-group").show();
		$("#NewsPaperCompany").parent(".form-group").show();
		$("#PressIDCard").parent(".form-group").show();
	}
	else {
		$("#PressReporterName").parent(".form-group").hide();
		$("#NewsPaperCompany").parent(".form-group").hide();
		$("#PressIDCard").parent(".form-group").hide();
	}
}
roleChangeformedia();


$(".menu-list,body").niceScroll({
	cursorwidth: "6px",
	bouncescroll: false
});
if ($(window).width() > 768) {
	$(".tableHrscroll").niceScroll({
		cursorwidth: "6px",
		bouncescroll: false,
		horizrailenabled: true
	});
}
