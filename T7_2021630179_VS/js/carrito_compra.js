document.getElementById('clearCart').addEventListener('click', async () => {
    const result = await clearCart();
    alert(result.message);
    loadCart();
});

document.getElementById('continueShopping').addEventListener('click', () => {
    window.location.href = 'compra_articulos.html';
});

async function loadCart() {
    const cartItemsDiv = document.getElementById('cartItems');
    cartItemsDiv.innerHTML = '';

    const cartItems = await getCartItems();
    cartItems.forEach(item => {
        const itemDiv = document.createElement('div');
        itemDiv.classList.add('cart-item');

        itemDiv.innerHTML = `
            <img src="data:image/png;base64,${item.photo}" alt="${item.name}">
            <h2>${item.name}</h2>
            <p>${item.description}</p>
            <p>Precio: ${item.price}</p>
            <p>Cantidad: ${item.quantity}</p>
            <p>Costo: ${item.quantity * item.price}</p>
            <button onclick="removeArticleFromCart(${item.id})">Eliminar artículo</button>
        `;

        cartItemsDiv.appendChild(itemDiv);
    });
}

async function removeArticleFromCart(id) {
    const result = await removeArticleFromCart(id);
    alert(result.message);
    loadCart();
}

loadCart();
