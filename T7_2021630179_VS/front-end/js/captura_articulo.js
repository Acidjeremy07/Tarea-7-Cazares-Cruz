document.getElementById('captureForm').addEventListener('submit', async (event) => {
    event.preventDefault();

    const name = document.getElementById('name').value;
    const description = document.getElementById('description').value;
    const price = document.getElementById('price').value;
    const quantity = document.getElementById('quantity').value;
    const photo = document.getElementById('photo').files[0];

    const reader = new FileReader();
    reader.onloadend = async () => {
        const base64Photo = reader.result.split(',')[1];

        const article = {
            name,
            description,
            price,
            quantity,
            photo: base64Photo
        };

        try {
            const result = await createArticle(article);
            alert(result.message);
        } catch (error) {
            console.error('Error:', error);
            alert('Error al guardar el artículo');
        }
    };
    reader.readAsDataURL(photo);
});
