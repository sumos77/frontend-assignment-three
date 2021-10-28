const form = document.querySelector('form');
const notesList = document.querySelector('#notes');
const pendingTasks = document.querySelector('.pendingTasks');
const footer = document.querySelector('.footer');
const toggleButton = document.getElementById('toggle-all')
const clearButton = document.getElementById('clear-storage');
const clearCompletedTasksButton = document.getElementById('clear-all-completed');
const viewAll = document.getElementById('view-all');
const viewActive = document.getElementById('view-active');
const viewCompleted = document.getElementById('view-completed');

//idCounter is a counter that makes sure each note gets a unique id
let idCounter = 0;
//listOfNotes contains an array of all notes, both active and completed,
let listOfNotes = [];

//reads from local storage and displays it
readLocalStorage();
drawToDo(listOfNotes);
renderActiveTasksCount();

document.getElementById('note').focus();

//an eventListener that checks for any changes to the url, and when it changes
//to the right one it does the same thing as the buttons.
window.addEventListener('hashchange', function() {
    drawToDo(listOfNotes);
  }, false);

function drawToDo(view){
    
    const atLeastOneCompleted = listOfNotes.some(note => note.noteCompleted);

    if (atLeastOneCompleted){
        clearCompletedTasksButton.style.visibility='visible';
    }
    else{
        clearCompletedTasksButton.style.visibility='hidden';
    }

    notesList.replaceChildren();

    //depending on where we are in the url, display the notes that are active, completed or all (default)
    //the .selected class changes to the currently active hash as well, and that is for css purposes
    if (location.hash == '#/active') {
        view = listOfNotes.filter((item) => item.noteCompleted == false);
        const deleteSelected = document.querySelector('.selected');
        deleteSelected.classList.remove('selected');
        viewActive.className = 'selected';   
    }
    else if (location.hash == '#/completed' ) {
        view = listOfNotes.filter((item) => item.noteCompleted == true);
        const deleteSelected = document.querySelector('.selected');
        deleteSelected.classList.remove('selected');
        viewCompleted.className = 'selected';
    }
    else {
        const deleteSelected = document.querySelector('.selected');
        deleteSelected.classList.remove('selected');
        viewAll.className = 'selected';
    }

    if(listOfNotes.length==0){
        toggleButton.style.visibility = 'hidden';
        footer.style.display = 'none';
    }
    else{
        toggleButton.style.visibility = 'visible';
        footer.style.display = 'flex';
    }
    
    view.forEach(item => {
        
        const li = document.createElement('li');
        li.classList.add('notediting');
        const label = document.createElement('label');

        notesList.append(li);
        li.append(label);

        label.textContent = item.noteText;
        label.setAttribute('name', 'savedNote');

        const completeCheckBox = document.createElement('input');
        completeCheckBox.type = 'checkbox';
        completeCheckBox.name = item.noteId;
        completeCheckBox.className = 'complete-checkbox';

        if(item.noteCompleted == true){
            form.reset();
            completeCheckBox.checked=true;
        }

        completeCheckBox.onclick = event => {

            //completedItem gets the object that corresponds to the todoNote who's checkbox was clicked
            completedItem = view.filter(n => n.noteId == completeCheckBox.name)[0];
            //flips noteCompleteds value (true or false)
            completedItem.noteCompleted = !completedItem.noteCompleted;

            renderActiveTasksCount();
            writeLocalStorage();
            drawToDo(view);
        }

        li.prepend(completeCheckBox);

        toggleButton.onclick = event => {
            
            const atLeastOneActive = listOfNotes.some(note => !note.noteCompleted);

            if (atLeastOneActive) {
                listOfNotes.forEach(turnFalseTrue => {
                    turnFalseTrue.noteCompleted = true;
                })
            }
            else{
                listOfNotes.forEach(turnTrueFalse => {
                    turnTrueFalse.noteCompleted = false;
                })
            }
            
            renderActiveTasksCount();
            writeLocalStorage();
            drawToDo(view);
        }

        const removeButton = document.createElement('button');
        removeButton.type = 'button';
        removeButton.name = item.noteId;
        removeButton.value = item.noteOrder;
        removeButton.className = 'remove-button';
        removeButton.textContent = '❌';

        removeButton.onclick = event => {

            //removes the button that corresponds to that particular button value
            listOfNotes.splice(parseInt(removeButton.value), 1);

            //loops through the remaining objects in listOfNotes and giving the new order values
            for (let i=0; i<listOfNotes.length; i++){
                listOfNotes[i].noteOrder = i;
            }

            writeLocalStorage()
            localStorage.removeItem(removeButton.name);
            drawToDo(view);
            renderActiveTasksCount();
        };
        
        li.append(removeButton);

        const editInput = document.createElement('input');
        editInput.type = 'text';
        editInput.className = 'note-edit';
        editInput.value = item.noteText;
        editInput.style.display = 'none';
        editInput.name = item.noteId;
        editInput.id = 'editNote';
        li.append(editInput);

        //When a li element is double clicked the label dissappears and an input appears and gets focus
        li.ondblclick = event => {
            editInput.style.display = 'flex';
            editInput.focus();
            label.style.display = 'none';
            li.classList.add('editing');
            li.classList.remove('notediting');
        }

        //Sets the label to the textcontent of the input when input loses focus
        li.addEventListener('focusout', (event) => {
            if (li.className != 'editing'){
                return;
            }
            else {
                if (editInput.value.trim() != ''){
                    newText = editInput.value.trim();
                    label.textContent = newText;
                    updatedNote = listOfNotes.filter(n => n.noteId == removeButton.name)[0];
                    updatedNote.noteText = newText;
                }
                else {
                    editInput.value = item.noteText;
                }
                editInput.style.display = 'none';
                label.style.display = 'flex';
                li.classList.remove('editing');
                li.classList.add('notediting');
                writeLocalStorage();
            }
        })

        //Sets the label to the textcontent of the input when the enter key is pressed
        li.addEventListener('keydown', (event) =>{
            if (event.key == 'Enter') {
                if (li.className != 'editing'){
                    return;
                }
                else {
                    editInput.blur();
                }
            }
        })

        //If escape key is pressed it reverts to the previous value
        li.addEventListener('keydown', (event) =>{
            if (event.key == 'Escape') {
                editInput.value = label.textContent;
                editInput.blur();
            }
        })
    })

};

function writeLocalStorage(){

    //turns each item in listOfNotes into strings so localstorage can store them
    listOfNotes.forEach(item => {
        localStorage.setItem(item.noteId, JSON.stringify(item));
    })

    //uses the noteIds as keys so we can find them in the localstorage
    let noteIdStorage = listOfNotes.map(n => n.noteId);
    localStorage.setItem('todo-keys', JSON.stringify(noteIdStorage));
}

function readLocalStorage(){
    //gets the keys from the localstorage
    let noteIdStorage = JSON.parse(localStorage.getItem('todo-keys'));

    //checks that localstorage contains any keys, and if it does use those keys to obtain the right objects
    //from localstorage and store them in listOfNotes
    if(noteIdStorage==null || noteIdStorage.length==0){
        listOfNotes = [];
    }
    else{
        listOfNotes = noteIdStorage.map(n => JSON.parse(localStorage.getItem(n)));
        idCounter = noteIdStorage.pop() + 1;
    }
}

//creates and array that cointains the object todoNote
form.onsubmit = event => {
    event.preventDefault();

    //removes any empty characters before and after
    const textInput = form.elements.note.value.trim();

    //returns early if the input string is empty or contains only spaces
    if(textInput == ''){
        return;
    }

    let itemOrder = listOfNotes.length;
    let todoNote = {
        noteText: textInput, 
        noteId: idCounter,
        noteCompleted: false,
        noteOrder: itemOrder
    };

    listOfNotes.push(todoNote);

    writeLocalStorage();
    drawToDo(listOfNotes);
    form.reset();

    idCounter++;

    renderActiveTasksCount();
};

//displays the current active tasks/notes
function renderActiveTasksCount(){
    activeTasksCount = 0;
    for (let i=0; i < listOfNotes.length; i++){
        
        if(!listOfNotes[i].noteCompleted){
            activeTasksCount++;
        }
    }

    if(activeTasksCount === 1){
        pendingTasks.textContent = activeTasksCount + ' item left';
    }
    else{
        pendingTasks.textContent = activeTasksCount + ' items left';
    }
};

//Creates deletedNotes for the purpose of knowing which ones to delete from
//localStorage. The listOfNotes is simply updated with a .filter.
clearCompletedTasksButton.onclick = event => {
    let deletedNotes = listOfNotes.filter((item) => item.noteCompleted == true);
    listOfNotes = listOfNotes.filter((item) => item.noteCompleted == false);

    deletedNotes.forEach(item => {
        localStorage.removeItem(item.noteId);
    })

    drawToDo(listOfNotes);
    writeLocalStorage();
};  