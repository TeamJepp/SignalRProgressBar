const xhttp = new XMLHttpRequest();
const taskAction = "checkOut";
const redirectUrl = "/ProgressBar/Success";
const hubUrl = "/progressHub";
const initProgressBar = "initProgressBar";
const updateProgressBar = "updateProgressBar";
const completed = "completed";
const exception = "exception";
const connected = "connected";
const iReconnnect = 5;
let iReTry = 0;
const signalRConnection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .build();
const stripeKey = document.getElementById('StripePublishableKey').value;
const stripe = Stripe(stripeKey);
const elements = stripe.elements();
const style = {
    base: {
        color: '#32325d',
        fontFamily: 'sans-serif',
        fontSmoothing: 'antialiased',
        fontSize: '16px',
        '::placeholder': {
            color: '#aab7c4'
        }
    },
    invalid: {
        color: '#fa755a',
        iconColor: '#fa755a'
    }
};
const card = elements.create('card', { style: style });

// Add an instance of the card Element into the `card-element` <div>.
card.mount('#card-element');

// Handle real-time validation errors from the card Element.
card.addEventListener('change', function (event) {
    let displayError = document.getElementById('card-errors');
    if (event.error) {
        displayError.textContent = event.error.message;
    } else {
        displayError.textContent = '';
    }
});

//Adds loading-text back to jquery (for the button)
(function ($) {
    $.fn.button = function (action) {
        if (action === 'loading' && this.data('loading-text')) {
            this.data('original-text', this.html()).html(this.data('loading-text')).prop('disabled', true);
        }
        if (action === 'reset' && this.data('original-text')) {
            this.html(this.data('original-text')).prop('disabled', false);
        }
        if (action === 'complete' && this.data('complete-text')) {
            this.data('original-text', this.html()).html(this.data('complete-text')).prop('disabled', true);
        }
    };
}(jQuery));

// Open SignalR Connection
async function openConnection() {

    signalRConnection.on(connected, () => {
        $('#msgerror-row').hide();
        console.log('connected');
        $("#checkOutButton").removeAttr("disabled");
    });

    signalRConnection.on(initProgressBar, (info) => {
        $("#notification").show();
        setProgress(info);
    });

    signalRConnection.on(updateProgressBar, (info) => {
        setProgress(info);
    });

    signalRConnection.on(completed, (info) => {
        $('#checkOutButton').button('complete');
        $("#headerMessage").addClass('text-success');
        $("#headerMessage").html(info.message);
        $("#percentage").text('100');
        $('.progress-bar').css('width', '100%').attr('aria-valuenow', 100);
        window.setTimeout(function () {
            window.location.href = redirectUrl;
        }, 5000);
    });

    signalRConnection.on(exception, (info) => {
        console.log(info.message);
        $('#checkOutButton').button('reset');
        $("#headerMessage").addClass('text-danger');
        $('#msgerror-row').show();
        $('#msgerror').html(info.message);
    });

    await signalRConnection.start()
        .catch(function (err) {
            $('#msgerror-row').show();
            console.log("Error Connecting: " + err.toString());
            if (iReTry < iReconnnect) {
                iReTry++;
                console.log("Attempting to Reconnect");
                $('#msgerror').html("Error while establishing connection.  Attempting to reconnect...");
                setTimeout(() => openConnection(), 5000);
            } else {
                console.log("Exhausted all attempts to reconnect...");
                $('#msgerror').html("Could not reconnect after " + iReTry + " Attempts.  Please try again later.");
            }
            return;
        });
}

signalRConnection.onclose(async () => {
    $("#checkOutButton").prop("disabled", true);
    $('#msgerror-row').show();
    $('#msgerror').html("Lost Connection.  Attempting To Reconnect...");
    await openConnection();
});

function resetUI() {
    $("#notification").hide();
    $("#msgerror-row").hide();
    $('#checkOutButton').button('reset');
    $("#headerMessage").removeClass('text-danger');
}

function setProgress(info) {
    $("#headerMessage").html(info.message);
    $("#percentage").text(info.pct);
    $('.progress-bar').css('width', info.pct + '%').attr('aria-valuenow', info.pct);
}

// Automatically establish a connection upon loading. 
openConnection();

xhttp.onreadystatechange = function () {
    if (xhttp.readyState === XMLHttpRequest.DONE) {
        if (xhttp.status !== 200) {
            alert('There was an error processing the AJAX request.');
        }
    }
};

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("checkOutButton").onclick = function () {
        $("#headerMessage").removeClass('text-danger');
        $("#msgerror-row").hide();
        $('#checkOutButton').button('loading');
        event.preventDefault();

        stripe.createToken(card).then(function (result) {
            if (result.error) {
                // Inform the user if there was an error.
                let errorElement = document.getElementById('card-errors');
                errorElement.textContent = result.error.message;
                resetUI();
            } else {
                // Send the token to server.
                startTask(result.token.id);
            }
        });
    };
});

function startTask(token) {
    xhttp.open('POST', taskAction, true);
    xhttp.setRequestHeader("RequestVerificationToken",
        document.getElementById('RequestVerificationToken').value);
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    let params = JSON.stringify({ stripeToken: token });
    xhttp.send(params);
}
