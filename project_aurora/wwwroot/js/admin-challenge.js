document.addEventListener("DOMContentLoaded", () => {

    const popup = document.getElementById("challengePopup");
    const openButton = document.getElementById("createChallengeBtn");
    const cancelButton = document.getElementById("cancelChallengeBtn");

    if (!popup || !openButton || !cancelButton) return;

    openButton.addEventListener("click", () => {
        popup.classList.add("active");
    });

    cancelButton.addEventListener("click", () => {
        popup.classList.remove("active");
    });

});