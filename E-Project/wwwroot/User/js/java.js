// preloader
window.addEventListener("load", function () {
    setTimeout(function () {
        const loader = document.getElementById("preloader");
        loader.classList.add("hide");
    }, 500); // 0.5s delay only
});
// preloader


// techtination
 var swiper = new Swiper(".reviewSwiper", {
    slidesPerView: 4,
    spaceBetween: 20,
    loop: true,
    autoplay: {
      delay: 3000,
      disableOnInteraction: false,
    },
    navigation: {
      nextEl: ".swiper-button-next",
      prevEl: ".swiper-button-prev",
    },
    breakpoints: {
      0: { slidesPerView: 1 },
      576: { slidesPerView: 2 },
      992: { slidesPerView: 3 },
      1200: { slidesPerView: 4 },
    }
  });
// techtination




//cards javascripe for module
// Open Customize Modal with selected card details
function openCustomize(imageSrc, title, message) {
    document.getElementById("previewImage").src = imageSrc;
    document.getElementById("cardTitle").value = title;
    document.getElementById("cardMessage").value = message;

    var customizeModal = new bootstrap.Modal(document.getElementById("customizeModal"));
    customizeModal.show();
}

// Upload Image from System and Preview
function updateImageFromFile(event) {
    // Handle both event object and direct file parameter
    let file;
    if (event && event.target && event.target.files) {
        file = event.target.files[0];
    } else if (event && typeof event === 'object' && event.files) {
        file = event.files[0];
    } else {
        // Fallback: get file from the input element directly
        const input = document.getElementById('imageFile');
        if (input && input.files) {
            file = input.files[0];
        }
    }
    
    if (file) {
        let reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById("previewImage").src = e.target.result;
        }
        reader.readAsDataURL(file);
    }
}

// Move to Send Modal
function openSendModal() {
    var customizeModal = bootstrap.Modal.getInstance(document.getElementById("customizeModal"));
    customizeModal.hide();

    var sendModal = new bootstrap.Modal(document.getElementById("sendModal"));
    sendModal.show();
}

// ? Send Card with Validation
function sendCard() {
    let emailInput = document.getElementById("recipientEmail");
    let emailError = document.getElementById("emailError");
    let email = emailInput.value.trim();

    // Regex for email validation
    let emailPattern = /^[^\s]+[^\s]+\.[^\s]+$/;

    if (email === "" || !emailPattern.test(email)) {
        emailError.classList.remove("d-none");   // show error text
        emailInput.classList.add("is-invalid");  // red border
        emailInput.classList.remove("is-valid");
        return;
    } else {
        emailError.classList.add("d-none");      // hide error text
        emailInput.classList.remove("is-invalid");
        emailInput.classList.add("is-valid");    // green border ?
    }

    // Success Message
    alert("?? Card Sent Successfully!");

    // Close modal after sending
    var sendModal = bootstrap.Modal.getInstance(document.getElementById("sendModal"));
    sendModal.hide();

    // Reset input
    emailInput.value = "";
    emailInput.classList.remove("is-valid");
}
//cards javascripe for module




// Drag and Drop Functions start
function handleDrop(event) {
    event.preventDefault();
    const dragZone = event.currentTarget;
    dragZone.classList.remove('drag-over');

    const files = event.dataTransfer.files;
    if (files.length > 0) {
        const file = files[0];
        if (file.type.startsWith('image/')) {
            updateImageFromFile({ target: { files: [file] } });
        } else {
            alert('Please drop an image file.');
        }
    }
}

function handleDragOver(event) {
    event.preventDefault();
}

function handleDragEnter(event) {
    event.preventDefault();
    const dragZone = event.currentTarget;
    dragZone.classList.add('drag-over');
}

function handleDragLeave(event) {
    event.preventDefault();
    const dragZone = event.currentTarget;
    dragZone.classList.remove('drag-over');
}

// Add drag and drop styles
function addDragDropStyles() {
    if (!document.getElementById('dragDropStyles')) {
        const style = document.createElement('style');
        style.id = 'dragDropStyles';
        style.textContent = `
          .drag-drop-zone {
            border: 2px dashed #dee2e6;
            border-radius: 8px;
            padding: 30px 20px;
            text-align: center;
            background-color: #f8f9fa;
            transition: all 0.3s ease;
            cursor: pointer;
            min-height: 120px;
            display: flex;
            align-items: center;
            justify-content: center;
          }

          .drag-drop-zone:hover {
            border-color: #007bff;
            background-color: #e7f3ff;
          }

          .drag-drop-zone.drag-over {
            border-color: #28a745;
            background-color: #d4edda;
            transform: scale(1.02);
          }

          .drag-drop-content {
            pointer-events: none;
          }

          .drag-drop-zone .btn {
            pointer-events: auto;
          }

          .drag-drop-zone .fa-cloud-upload-alt {
            color: #6c757d;
            transition: color 0.3s ease;
          }

          .drag-drop-zone:hover .fa-cloud-upload-alt {
            color: #007bff;
          }

          .drag-drop-zone.drag-over .fa-cloud-upload-alt {
            color: #28a745;
          }
        `;
        document.head.appendChild(style);
    }
}

// Initialize drag and drop styles when page loads
document.addEventListener('DOMContentLoaded', function () {
    addDragDropStyles();
});
// Drag and Drop Functions end












