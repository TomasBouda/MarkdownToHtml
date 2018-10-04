/*
==========================
Vertical Responsive Menu
==========================
*/

'use strict';

var tid = setInterval(function () {
	if (document.readyState !== 'complete') return;
	clearInterval(tid);

	var querySelector = document.querySelector.bind(document);

	var nav = document.querySelector('.vertical_nav');
	var wrapper = document.querySelector('.wrapper');

	var menu = document.getElementById("js-menu");

	// Toggle menu click
	querySelector('.toggle_menu').onclick = function () {
		nav.classList.toggle('vertical_nav__opened');

		wrapper.classList.toggle('toggle-content');
	};
}, 100);