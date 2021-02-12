/*
 *  invoke hub function when sending ajax to controller
 *  call joinRoom onclick on room name
 */




var connection = new signalR.HubConnectionBuilder()
    .withUrl('/chatHub')
    .build();

var _connectionId = '';

connection.on('ReceiveMessage', function (data) {
    console.log(data);
    console.log('2****2');
    var message = document.createElement('div');
    message.classList.add('message');

    console.log('message created');

    var header = document.createElement('header');
    console.log('new header');
    //header.val(data.name);
    header.appendChild(document.createTextNode(data.name));

    console.log('header created');

    var p = document.createElement('p');
    p.appendChild(document.createTextNode(data.text));

    console.log('p created');

    var footer = document.createElement('footer');
    footer.appendChild(document.createTextNode(data.timeSpan));

    console.log('footer created');

    message.appendChild(header);
    message.appendChild(p);
    message.appendChild(footer);

    //$('.chat-body').appendChild(message);
    console.log(message);
    document.getElementById('chatBody').appendChild(message);
});

var joinRoom = function () {
    var roomName = $('#roomName').val();
    console.log(roomName + ' room name');
    var url = '/Chat/JoinRoom/{' + _connectionId + '}/' + roomName;
    axios.post(url, null)
        .then(result => {
            console.log('Room Joined!', result);
        })
        .catch(err => {
            console.err('Failed to join Room!', result);
        });
}

//connection.start().then(() => {
//    connection.invoke('getConnectionId').then(function (connectionId) {
//        _connectionId = connectionId;
//        joinRoom();
//    });
//});

//connection.invoke('getConnectionId')
//    .then(function (connectionId) {
//        _connectionId = connectionId;
//        joinRoom();
//    });


connection.start()
    .then(function () {
        connection.invoke('getConnectionId')
            .then(function (connectionId) {
                _connectionId = connectionId;
                joinRoom();
            });
    })
    .catch(function (err) {
        console.log(err);
    });

var form = null;

$('#sendButton').on('click', function () {
    //sendMessage();
    event.preventDefault();
    console.log('1***1');
    var formMessage = $('#message').val();
    var formChatId = $('#chatId').val();
    var formRoomName = $('#roomName').val();
    var formUser = $('#user').val();

    var data = {
        message: formMessage,
        chatId: formChatId,
        roomName: formRoomName,
        user: formUser
    }

    var send = {
        message: formMessage,
        chatId: formChatId,
        roomName: formRoomName
    }

    var formData = new FormData();
    formData.append('message', formMessage);
    formData.append('chatId', formChatId);
    formData.append('roomName', formRoomName);

    console.log(send);
    $('#message').val('');

    $.post('/Chat/SendMessage', send).fail(function (date) {
        console.log('Failed to send message!');
        console.log(data);
    })
});

//var sendMessage = function (event) {
//    console.log('***');
//    event.preventDefault();

//    var formMessage = $('#message').val();
//    var formChatId = $('#chatId').val();
//    var formRoomName = $('#roomName').val();
//    //var formUser = $('#user');

//    var send = {
//        chatId: formChatId,
//        message: formMessage,
//        roomName: formRoomName
//    }


//    $('#message').val('');

//    $.post('/Chat/SendMessage', send, function () {
//        console.log('$post success');
//    })
//}