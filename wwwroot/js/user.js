let uri = '/User';
let userRole = '';
let usersItems = [];

function getToken() {
    return sessionStorage.getItem("token");
}

// פונקציה לבדוק את הרשאות המשתמש
function checkUserRole() {
    const token = getToken();
    if (token) {
        const payload = JSON.parse(atob(token.split('.')[1]));
        userRole = payload.type;
    }
}

function getItems() {
    checkUserRole();
    const token = getToken();
    let userUri = userRole === 'Admin' ? '/User/GetAll' : '/User/Get';

    fetch(userUri, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Authorization': token
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Failed to fetch items. Status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            _displayItems(data);
        })
        .catch(error => console.error(' Unable to get items.', error));
}

// פונקציה להצגת הטופס להוספת משתמש
function toggleAddForm() {
    const addForm = document.getElementById('addForm');
    addForm.style.display = addForm.style.display === 'none' || addForm.style.display === '' ? 'block' : 'none';
}

// פונקציה להוספת משתמש
function addUser() {
    const token = getToken();
    const addUsername = document.getElementById('add-username').value.trim();
    const addPassword = document.getElementById('add-password').value.trim();
    const addEmail = document.getElementById('add-email').value.trim();

    const newUser = {
        username: addUsername,
        password: addPassword,
        email: addEmail,
        role: "User"
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': token  // שליחת ה-token ב-headers
        },
        body: JSON.stringify(newUser)
    })
        .then(response => response.json())
        .then(() => {
            getItems();
            document.getElementById('add-username').value = '';
            document.getElementById('add-password').value = '';
            document.getElementById('add-email').value = '';
        })
        .catch(error => console.error('Unable to add user.', error));
    closeInputAdd();
}

// פונקציה לסגירת טופס ההוספה
function closeInputAdd() {
    document.getElementById('addForm').style.display = 'none';
}

// פונקציה למחיקת משתמש
function deleteUser(id) {
    const token = getToken();
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': token  // שליחת ה-token ב-headers
        }
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete user.', error));
}

// פונקציה להציג את טופס העריכה למשתמש
function displayEditForm(id) {
    const users = Array.isArray(usersItems) ? usersItems : [usersItems];

    const item = users.find(item => item.id === id);

    document.getElementById('edit-username').value = item.userName;
    document.getElementById('edit-email').value = item.email;
    document.getElementById('edit-password').value = item.password;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-password').value = item.password;
    document.getElementById('editForm').style.display = 'block';
}

function getCurrentUserId() {
    const token = getToken();
    if (token) {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.id; // ה-ID של המשתמש המחובר
    }
    return null;
}

// פונקציה לעדכון פרטי משתמש
function updateUser() {
    const token = getToken();
    const itemId = document.getElementById('edit-id').value;

    // קבלת המשתמש מהרשימה הקיימת כדי לשמור על התפקיד
    const users = Array.isArray(usersItems) ? usersItems : [usersItems];
    const existingUser = users.find(user => user.id == itemId);
    let userRole = existingUser ? existingUser.role : 'User';

    // אם המשתמש עצמו מעדכן את עצמו, נשמור על ה-Role שלו
    if (parseInt(itemId, 10) === getCurrentUserId()) {
        userRole = userRole;
    }

    const item = {
        id: parseInt(itemId, 10),
        username: document.getElementById('edit-username').value.trim(),
        password: document.getElementById('edit-password').value.trim(),
        email: document.getElementById('edit-email').value.trim(),
        role: userRole
    };

    fetch(uri, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': token
        },
        body: JSON.stringify(item)
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to update user.', error));

    closeInput();
    return false;
}

// פונקציה לסגירת טופס העריכה
function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayItems(data) {
    if (userRole == "Admin")
        document.getElementById('addButton').style.display = 'block'

    const tBody = document.getElementById('userItems');
    tBody.innerHTML = '';

    const users = Array.isArray(data) ? data : [data];

    users.forEach(item => {
        let editButton = document.createElement('button');
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = document.createElement('button');
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteUser(${item.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        td1.appendChild(document.createTextNode(item.userName));

        let td2 = tr.insertCell(1);
        td2.appendChild(document.createTextNode(item.email));

        let td3 = tr.insertCell(2);
        td3.appendChild(document.createTextNode(item.password));

        let td4 = tr.insertCell(3);
        td4.appendChild(editButton);

        if (userRole === 'Admin') {
            let td5 = tr.insertCell(4);
            td5.appendChild(deleteButton);
        }
    });
    usersItems = data;
}

function initPage() {
    const token = getToken();
    if (!token)
        // אם אין טוקן, הפניה לדף הלוגין
        window.location.href = 'login.html';
    else
        document.body.classList.add('show');
}

// קריאה לפונקציה בעת טעינת הדף
document.addEventListener('DOMContentLoaded', initPage);