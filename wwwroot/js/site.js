$(document).ready(function () {
    //Define active page/link
    $("a").each(function () {
        if (this.href == window.location.href) {
            $(this).addClass("active_link");
        }
    });
});

function toggleMenuClick() {
    let toggleBtnIcon = document.querySelector('.toggle_btn i')
    let dropdownMenu = document.querySelector('.dropdown_menu')

    dropdownMenu.classList.toggle('open');
    let isOpen = dropdownMenu.classList.contains('open');

    toggleBtnIcon.classList = isOpen ? 'fa-solid fa-xmark' : 'fa-solid fa-bars';
}

