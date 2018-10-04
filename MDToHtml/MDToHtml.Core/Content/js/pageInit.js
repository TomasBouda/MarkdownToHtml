$(function () {
	//Initialization of treeview
	$('.vertical_nav ul').treed({ openedClass: 'fa-angle-down', closedClass: 'fa-angle-right' });

	$(".link").each(function () {
		var file = $(this).attr("data-file-path");

		$(this).click(function () {
			$("#page-container").load(file);

			if (file != location.href) {
				history.pushState({ page: file }, null, '?page=' + file);
			}

			showCurrentPageInTree(false);
		});
	});

	$("#page-container").on("click", ".link", function () {
		var file = $(this).attr("data-file-path");
		$("#page-container").load(file);

		if (file != location.href) {
			history.pushState({ page: file }, null, '?page=' + file);
		}
	});

	window.onpopstate = function (event) {
		loadPageByQueryParam();
	};

	//window.onbeforeunload = function (e) {
	//	var message = "Leave wiki?",
	//		e = e || window.event;
	//	// For IE and Firefox
	//	if (e) {
	//		e.returnValue = message;
	//	}

	//	// For Safari
	//	return message;
	//};

	loadPageByQueryParam();
	showCurrentPageInTree(true);
});