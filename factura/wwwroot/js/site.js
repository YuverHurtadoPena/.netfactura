document.getElementById("openModalBtn").addEventListener("click", function () {
    document.getElementById("myModal").style.display = "block";
});
document.getElementById("idGuardar").addEventListener("click", function () {
    document.getElementById("myModal").style.display = "block";
});

document.getElementById("closeModalBtn").addEventListener("click", function () {
    document.getElementById("myModal").style.display = "none";
});
document.getElementById("clearForm").addEventListener("click", function () {

    var formulario = document.getElementById("miFormulario");

    formulario.reset();
});
