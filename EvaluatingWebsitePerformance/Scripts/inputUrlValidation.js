function submit_form(form) {
    var inputUrl = document.getElementById("inputUrl").value;

    if (isValidUrl(inputUrl)) {
        form.submit();
        document.getElementById("message").innerHTML = '';
        document.getElementById("submitButton").disabled = true;
        document.getElementById("submitButton").innerHTML = `<span class="spinner-border" role="status" aria-hidden="true"></span> Loading...`;
    }
    else {
        document.getElementById("message").innerHTML = 'Invalid URL';
    }
}

function isValidUrl(urlString) {
    var pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
        '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
        '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
        '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
        '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
        '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
    return !!pattern.test(urlString);
}