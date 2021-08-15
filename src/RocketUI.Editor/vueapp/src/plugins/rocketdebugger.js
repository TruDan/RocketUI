import Vue from "vue";


class RocketDebugger extends EventTarget {

    static instance;

    static defaultOptions = {
        url: "ws://localhost:2345/",
        reconnectDelay: 2500,
        messageTimeout: 30 * 1000,
    };

    options;
    /** @type WebSocket */
    #socket;
    #reconnectTimeout;
    #messageSendQueue = [];
    #messageCallbackQueue = [];
    #messageId = 0;

    constructor(options) {
        super();
        RocketDebugger.instance = this;

        this.options = Object.assign({}, RocketDebugger.defaultOptions, options);

        this.createSocket();
        setInterval(this.processDeadQueue.bind(this), 1000);
    }

    processDeadQueue() {
        const now = new Date().getTime();
        for (let i = 0; i < this.#messageCallbackQueue.length; i++) {
            let message = this.#messageCallbackQueue[i];

            if ((now - message.timeSent) > this.options.messageTimeout) {
                console.warn('RocketWS', 'Timeout', message);

                this.#messageCallbackQueue.splice(i, 1);
                i--;
                message.errorCallback('timeout');
            }
        }
    }

    getStatus() {
        return this.#socket.readyState;
    }

    createSocket() {
        this.dispatchEvent(new Event('onStateChanged', { state: WebSocket.CONNECTING }));
        this.#socket = new WebSocket(this.options.url);
        this.#socket.addEventListener('close', this.onSocketClose.bind(this));
        this.#socket.addEventListener('open', this.onSocketOpen.bind(this));
        this.#socket.addEventListener('error', this.onSocketError.bind(this));
        this.#socket.addEventListener('message', this.onSocketMessage.bind(this));
    }

    // eslint-disable-next-line no-unused-vars
    onSocketOpen(event) {
        this.dispatchEvent(new Event('onStateChanged', { state: WebSocket.OPEN }));
        console.log('RocketWS', 'Connected.');
        if (this.#messageSendQueue.length > 0) {
            for (let i = 0; i < this.#messageSendQueue.length; i++) {
                if (!(this.#socket.readyState === WebSocket.OPEN)) return;

                let queueItem = this.#messageSendQueue[i];

                this.#messageSendQueue.splice(i, 1);
                i--;
                this.sendRaw(queueItem.message);
            }
        }
    }

    onSocketError(event) {
        this.dispatchEvent(new Event('onStateChanged', { state: WebSocket.CLOSED }));
        console.error('RocketWS', 'Error', event);
    }

    onSocketMessage(event) {
        console.log('RocketWS', 'Received', event.data);
        this.handleMessage(JSON.parse(event.data));
    }

    // eslint-disable-next-line no-unused-vars
    onSocketClose(event) {
        console.log('RocketWS', 'Disconnected.');
        if (this.options.reconnectDelay >= 0) {
            console.log('RocketWS', 'Reconnecting in ' + this.options.reconnectDelay + 'ms...');
            this.#reconnectTimeout = setTimeout(this.tryReconnect.bind(this), this.options.reconnectDelay);
        }
    }

    tryReconnect() {
        this.createSocket();
    }

    tryTakeFromQueue(messageId) {
        const queueItem = this.#messageCallbackQueue.find(x => x.id === messageId);
        this.#messageCallbackQueue = this.#messageCallbackQueue.filter(x => x.id !== messageId);
        return queueItem;
    }

    handleMessage(message) {
        if ('id' in message) {
            console.log("RocketWS", "Handle", message);

            // find matching messageID
            const queueItem = this.tryTakeFromQueue(message.id);
            if (queueItem !== null) {
                if(message.error) {
                    console.log("RocketWS", "Call Error Callback", queueItem);
                    queueItem.errorCallback(message.error);
                }
                else {
                    console.log("RocketWS", "Call Callback", queueItem);
                    queueItem.callback(message.data);
                }
                return;
            }
            else {
                console.warn("RocketWS", "HandleNotFound", message);
            }
        }

        this.handleUnqueuedMessage(message);
    }

    handleUnqueuedMessage(message) {
        console.warn("RocketWS", "Unhandled Message", message);
    }

    send(command, ...args) {
        return new Promise((resolve, reject) => {
            const message = {
                id: this.generateMessageId(),
                command: command,
                arguments: args,
            };

            this.#messageCallbackQueue.push({
                id: message.id,
                timeSent: new Date().getTime(),
                callback: (response) => {
                    console.log('RocketWS', 'Callback', message);
                    resolve(response);
                },
                errorCallback: (error) => {
                    console.error('RocketWS', 'Error Callback', message);
                    reject(error);
                }
            });

            this.sendRaw(message);
        });
    }

    sendRaw(message) {
        if (!(this.#socket.readyState === WebSocket.OPEN)) {
            this.#messageSendQueue.push({
                message: message,
                timeSent: new Date().getTime()
            });
            console.log('RocketWS', 'Queued Send', message);
            return;
        } else {
            this.#socket.send(JSON.stringify(message));
            console.log('RocketWS', 'Send', message);
        }
    }

    generateMessageId() {
        return this.#messageId++;
    }

    static install = (Vue, options) => {
        const ws = new RocketDebugger(options);

        Vue.prototype.$rocketws = ws;
        window.$rocketws = ws;
    }
}

Vue.use(RocketDebugger);

export default RocketDebugger.instance;
