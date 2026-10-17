document.addEventListener("click", function(event) {
    const navigationMenu = document.querySelector("#top-menu-links");
    const hamburgerIconButton = document.querySelector(".top-menu-nav a.icon")

    if (!navigationMenu.contains(event.target) && !hamburgerIconButton.contains(event.target) && navigationMenu.style.display != "none") {
        navigationMenu.style.display = "none";
    }
});

function toggleNavigationMenu() {
    const navigationMenu = document.getElementById("top-menu-links");

    if (navigationMenu.style.display == "flex") {
        navigationMenu.style.display = "none";
    } else {
        navigationMenu.style.display = "flex";
    }
}