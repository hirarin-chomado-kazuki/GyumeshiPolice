function createImgFromFile(htmlId) {
    const openFilePicker = () => {
        return new Promise(resolve => {
            let input = document.getElementById('file-input');
            if (!input) {
                input = document.createElement('input');
                input.id = 'file-input';
                input.type = 'file';
                input.accept = 'image/png,image/jpeg,image/gif';
                document.body.appendChild(input);
            }
            input.onchange = event => { resolve(event.target.files[0]); };
            input.click();
        });
    };

    (async () => {
        const file = await openFilePicker();
        const url = window.URL.createObjectURL(file);

        const img = document.createElement('img');
        img.id = htmlId;
        img.src = url;
        document.body.appendChild(img);
    })();
}

function getImageSrc(htmlId) {
    const img = document.getElementById(htmlId);
    if (img) {
        return img.getAttribute('src');
    }
    return null;
}