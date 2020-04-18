// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const dropDown = document.getElementById('nav-dropdown');
const btn = document.getElementById('nav-dropdown-btn');

function toggleDropDown() {
    if (dropDown.style.visibility === 'visible') {
        dropDown.style.visibility = 'hidden';
    } else {
        dropDown.style.visibility = 'visible';
    }
}

btn.addEventListener('click', toggleDropDown);