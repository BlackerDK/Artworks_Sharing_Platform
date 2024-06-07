var modal = document.getElementById("myModal");
var showModalBtn = document.getElementById("showModalBtn");
var closeModalBtn = document.getElementById("closeModalBtn");
showModalBtn.onclick = function () {
    modal.style.display = "block";
}
closeModalBtn.onclick = function () {
    modal.style.display = "none";
}
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

