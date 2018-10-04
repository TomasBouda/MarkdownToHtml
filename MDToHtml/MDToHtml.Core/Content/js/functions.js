$.fn.extend({
	treed: function (o) {
		var openedClass = 'fa-angle-down';
		var closedClass = 'fa-angle-right';

		if (typeof o != 'undefined') {
			if (typeof o.openedClass != 'undefined') {
				openedClass = o.openedClass;
			}
			if (typeof o.closedClass != 'undefined') {
				closedClass = o.closedClass;
			}
		};

		//initialize each of the top levels
		var tree = $(this);
		tree.addClass("tree");
		tree.find('li').has("ul").each(function () {
			var branch = $(this); //li with children ul
			branch.prepend("<i class='indicator fa " + closedClass + "' aria-hidden='true'></i>");
			branch.addClass('branch');
			branch.on('click', function (e) {
				if (this == e.target) {
					var icon = $(this).children('i:first');
					icon.toggleClass(openedClass + " " + closedClass);
					$(this).children().children().toggle();
				}
			})
			branch.children().children().toggle();
		});
		//fire event from the dynamically added icon
		tree.find('.branch .indicator').each(function () {
			$(this).on('click', function () {
				$(this).closest('li').click();
			});
		});
		//fire event to open branch if the li contains an anchor instead of text
		tree.find('.branch>a').each(function () {
			$(this).on('click', function (e) {
				$(this).closest('li').click();
				e.preventDefault();
			});
		});
		//fire event to open branch if the li contains a button instead of text
		tree.find('.branch>button').each(function () {
			$(this).on('click', function (e) {
				$(this).closest('li').click();
				e.preventDefault();
			});
		});
	}
});

function getPageFromQueryParam() {
	let urlParams = new URLSearchParams(window.location.search);
	let page = urlParams.get('page');

	return page;
}

function loadPageByQueryParam() {
	$("#page-container").load(getPageFromQueryParam());
}

function showCurrentPageInTree(unfoldTree) {
	var currentPage = getPageFromQueryParam();

	$(".link").each(function () {
		var file = $(this).attr("data-file-path");

		if (decodeURI(file) == currentPage) {
			$(this).css("text-decoration", "underline");

			if (unfoldTree) {
				$(this).parents("li.branch").each(function () {
					$(this).children("i.indicator").click();
				});
			}
		}
		else {
			$(this).css("text-decoration", "initial");
		}
	});
}