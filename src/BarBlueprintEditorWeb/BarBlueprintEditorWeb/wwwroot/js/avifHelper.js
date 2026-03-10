window.loadAvifAsImageData = async function (url) {
    return new Promise((resolve, reject) => {
        const img = new Image();
        img.CrossOrigin = "anonymous"; // important!
        img.src = url;
        img.onload = () => {
            const canvas = document.createElement('canvas');
            canvas.width = img.width;
            canvas.height = img.height;
            const ctx = canvas.getContext('2d');
            ctx.drawImage(img, 0, 0);
            const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
            resolve({
                width: canvas.width,
                height: canvas.height,
                data: Array.from(imageData.data) // JS array of RGBA
            });
        };
        img.onerror = reject;
    });
};