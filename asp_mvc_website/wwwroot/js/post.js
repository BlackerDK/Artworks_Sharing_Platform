
function upload() {

    const fileUploadInput = document.querySelector('.file-uploader');
  
    /// Validations ///
  
    if (!fileUploadInput.value) {
      return;
    }
  
    // using index [0] to take the first file from the array
    const image = fileUploadInput.files[0];
  
    if (!image.type.includes('image')) {
      return alert('Only images are allowed!');
    }
  
    // check if size (in bytes) exceeds 10 MB
    if (image.size > 10_000_000) {
      return alert('Maximum upload size is 10MB!');
    }
  
    /// Display the image on the screen ///
  
    const fileReader = new FileReader();
    fileReader.readAsDataURL(image);
  
    fileReader.onload = (fileReaderEvent) => {
      const profilePicture = document.querySelector('.profile-picture');
      profilePicture.style.backgroundImage = `url(${fileReaderEvent.target.result})`;
    }
  
    // upload image to the server or the cloud
  }
  
var modal = document.getElementById("PostModal");
var p_modal = document.getElementById("P_Modal"); 



var txtTitle = document.getElementById("title");
var txtDescription = document.getElementById("description");
var txtPrice = document.getElementById("price");
var pictureImaage = document.getElementById("p_image");

//var urlImage = document.getElementById("image").value;

var _clo_mo_btn = document.getElementById("CloseModalBtn");
_clo_mo_btn.addEventListener('click', function () {
    txtTitle.value = '';
    txtDescription.value = '';
    txtPrice.value = '';
    pictureImaage.style.background = '#252525';
});


document.getElementById("goPackageBtn").addEventListener("click", function () {
    
    window.location.href = '/Package/Index';
});
document.getElementById("closePackage").addEventListener("click", function () {
    p_modal.style.display = "none";
});

// Get the input element
const priceInput = document.getElementById('price');
const priceValidationMessage = document.getElementById('priceValidationMessage');
const titleValidationMessage = document.getElementById('titleValidationMessage');
const descriptionValidationMessage = document.getElementById('descriptionValidationMessage');
const catelogyValidationMessage = document.getElementById('catelogyValidationMessage');
const imageValidationMessage = document.getElementById('imageValidationMessage');


const postBtn = document.getElementById('postBtn');

var aiBtn = document.getElementById("checkAi-btn");
aiBtn.addEventListener('click', function () {
    var imageFile = document.querySelector(".file-uploader").files[0];
    var loadingpage = document.getElementById("loading-load");
    var isValid = true; 
    if (!imageFile) {
        imageValidationMessage.style.display = 'block';
        isValid = false;
    } else {
        imageValidationMessage.style.display = 'none';
    }
    loadingpage.style.display = 'block';
    if (isValid) {
        var formData = new FormData();
        formData.append("File", imageFile);
        fetch("/ImageAI/loadAI", {
            method: "POST",
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                document.getElementById('description').value = data.answer;
                loadingpage.style.display = 'none';
            })
            .catch(error => console.error('Error:', error));
    }

});


postBtn.addEventListener('click', function () {
    var title = document.getElementById("title").value.trim();
    var description = document.getElementById("description").value.trim();
    var price = document.getElementById("price").value.trim();
    var imageFile = document.querySelector(".file-uploader").files[0];
    var customItemInput = document.getElementById('categoryCustom').value.trim();
    var categorySelect = document.getElementById('category').value;
    var isValid = true; 



    // Validate title
    if (title ==='') {
        titleValidationMessage.style.display = 'block';
        isValid = false;
    } else {
        titleValidationMessage.style.display = 'none';
    }

    // Validate description
    if (description === '') {
        descriptionValidationMessage.style.display = 'block';
        isValid = false;
    } else {
        descriptionValidationMessage.style.display = 'none';
    }      

    // Validate price (must be a number)
    if (isNaN(price) || price === '' || price <= 1000 || price >= 1000000000) {
        priceValidationMessage.style.display = 'block';
        isValid = false;
    } else {
        priceValidationMessage.style.display = 'none';
    }

    // Validate image
    if (!imageFile) {
        imageValidationMessage.style.display = 'block';
        isValid = false;
    } else {
        imageValidationMessage.style.display = 'none';
    }

    // Validate selected item
    if (categorySelect === 'other' && customItemInput === '') {
        catelogyValidationMessage.style.display = 'block';
        isValid = false;
    } else {
        catelogyValidationMessage.style.display = 'none';
    }
    var category = categorySelect;
    var isNew = false;
    if (categorySelect === 'other') {
        category = customItemInput;
        isNew = true;
    }


    // Proceed with form submission if all fields are valid
    if (isValid) {
        // Create FormData object
        var formData = new FormData();
        formData.append("Title", title);
        formData.append("Description", description);
        formData.append("Price", price);
        formData.append("File", imageFile);
        formData.append("Category", category);
        formData.append("IsNewCategory", isNew);

        // Send POST request to controller endpoint
        fetch("/Shop/Post", {
            method: "POST",
            body: formData
        })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
            .then(data => {
                if (data.success) {

                    document.getElementById('title').value = '';
                    document.getElementById('description').value = '';
                    document.getElementById('price').value = '';
                    document.getElementById('category').value = 'other';
                    document.getElementById('categoryCustom').value = '';
                    document.querySelector('.file-uploader').value = '';
                    document.querySelector('.profile-picture').style.backgroundImage = '';

                    var noti = document.getElementById("SuccessNoti");
                    modal.style.display = 'none';
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();

                    noti.style.display = 'block';
                    setTimeout(function () {
                        noti.style.display = 'none';
                        window.location.reload();
                    }, 3000);

                }
            })
            .catch(error => {
                // Handle error
                console.error('Error:', error);
            });
    }
});

document.getElementById('category').addEventListener('change', function () {
    var select = this;
    var customItemDiv = document.getElementById('customItemDiv');
    var customItemInput = document.getElementById('categoryCustom');
    if (select.value === 'other') {
        customItemDiv.style.display = 'block';
    } else {
        customItemDiv.style.display = 'none';
        customItemInput.value = '';
    }
});



