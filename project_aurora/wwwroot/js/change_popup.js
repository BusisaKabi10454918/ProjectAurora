const popup = document.getElementById("usernamePopup");

document.getElementById("changeUsernameBtn").addEventListener("click", () => {
    popup.classList.add("active");
});

document.getElementById("cancelUsernameBtn").addEventListener("click", () => {
    popup.classList.remove("active");
});