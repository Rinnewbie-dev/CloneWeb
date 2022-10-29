let onlineCount = document.querySelector('span.online-count');
let updateCountCallback = function (message) {
    if (!message) return;
    console.log('updateCount = ' + message);
    if (onlineCount) onlineCount.innerText = message;
};

function onConnectionError(error) {
    if (error && error.message) console.error(error.message);
}

let countConnection = new signalR.HubConnectionBuilder().withUrl('/onlinecount').build();
countConnection.on('updateCount', updateCountCallback);
countConnection.onclose(onConnectionError);
countConnection.start()
    .then(function () {
        console.log('OnlineCount Connected');
    })
    .catch(function (error) {
        console.error(error.message);
    });