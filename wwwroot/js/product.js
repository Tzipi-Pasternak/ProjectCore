let uri = '/jewelry';
let jewelryItems = [];

// קבלת ה-token מה-localStorage
function getToken() {
    return localStorage.getItem("token"); 
}

// פונקציה להחזרת פריטים מהממשק
function getItems() {
    const token = getToken();  
    fetch(uri, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Authorization': token 
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch items');
            }
            return response.json();
        })
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function toggleAddForm() {
    const addForm = document.getElementById('addForm');
    if (addForm.style.display === 'none' || addForm.style.display === '') {
        addForm.style.display = 'block';
    } else {
        addForm.style.display = 'none';
    }
}

// פונקציה להוספת פריט חדש
function addItem() {
    const token = getToken();  
    const addNameTextbox = document.getElementById('add-name');
    const addPriceTextbox = document.getElementById('add-price');
    const addCategoryTextbox = document.getElementById('add-category');

    const item = {
        name: addNameTextbox.value.trim(),
        price: parseFloat(addPriceTextbox.value),
        category: addCategoryTextbox.value.trim()
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': token  
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(() => {
            getItems();
            addNameTextbox.value = '';
            addPriceTextbox.value = '';
            addCategoryTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
    closeInputAdd();
}

function closeInputAdd() {
    document.getElementById('addForm').style.display = 'none';
}

// פונקציה למחיקת פריט
function deleteItem(id) {
    const token = getToken(); 
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': token  
        }
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

// פונקציה להציג את טופס העריכה
function displayEditForm(id) {
    const item = jewelryItems.find(item => item.id === id);

    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-price').value = item.price;
    document.getElementById('edit-category').value = item.category;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('editForm').style.display = 'block';
}

// פונקציה לעדכון פריט
function updateItem() {
    const token = getToken(); 
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        name: document.getElementById('edit-name').value.trim(),
        price: parseFloat(document.getElementById('edit-price').value),
        category: document.getElementById('edit-category').value.trim()
    };

    fetch(`${uri}/${itemId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': token  
        },
        body: JSON.stringify(item)
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to update item.', error));

    closeInput();
    return false;
}

// פונקציה לסגירת טופס העריכה
function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

// פונקציה לעדכון מונה הפריטים
function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'jewelry item' : 'jewelry items';
}

// פונקציה להצגת המוצרים בטבלה
function _displayItems(data) {
    const tBody = document.getElementById('jewelryItems');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        td1.appendChild(document.createTextNode(item.name));

        let td2 = tr.insertCell(1);
        td2.appendChild(document.createTextNode(item.price));

        let td3 = tr.insertCell(2);
        td3.appendChild(document.createTextNode(item.category));

        let td4 = tr.insertCell(3);
        td4.appendChild(editButton);

        let td5 = tr.insertCell(4);
        td5.appendChild(deleteButton);
    });

    jewelryItems = data;
}

const logOut = () => {
    localStorage.removeItem("token");
    initPage();
}

function initPage() {
    const token = getToken();
    if (!token) {
        window.location.href = 'login.html';
        return;
    }
    try {
        const tokenPayload = JSON.parse(atob(token.split('.')[1]));
        const expirationTime = tokenPayload.exp * 1000;
        const currentTime = Date.now();
        if (currentTime > expirationTime) {
            window.location.href = 'login.html';
        } else {
            document.body.classList.add('show');
        }
    } catch (error) {
        window.location.href = 'login.html';
    }
}

// קריאה לפונקציה בעת טעינת הדף
document.addEventListener('DOMContentLoaded', initPage);
