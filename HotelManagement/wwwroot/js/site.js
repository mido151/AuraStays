// AuraStays - Main JavaScript File

// Hotel Search Functionality
function initializeHotelSearch() {
    const searchInput = document.getElementById('hotel-search');
    const hotelGrid = document.getElementById('hotel-grid');

    if (!searchInput || !hotelGrid) return;

    const hotelCards = Array.from(hotelGrid.children);

    searchInput.addEventListener('input', (e) => {
        const searchTerm = e.target.value.toLowerCase().trim();

        hotelCards.forEach(card => {
            const hotelName = card.dataset.name?.toLowerCase() || '';
            const hotelLocation = card.dataset.location?.toLowerCase() || '';
            const isVisible = hotelName.includes(searchTerm) || hotelLocation.includes(searchTerm);

            card.style.display = isVisible ? '' : 'none';
        });
    });
}

// Hotel Details Slideshow
function initializeSlideshow() {
    const slideshowElement = document.getElementById('hotel-slideshow');

    if (!slideshowElement) return;

    const imagesData = slideshowElement.dataset.images;

    if (!imagesData) return;

    try {
        const images = JSON.parse(imagesData);
        let currentImageIndex = 0;

        // Change background image every 5 seconds
        setInterval(() => {
            currentImageIndex = (currentImageIndex + 1) % images.length;
            slideshowElement.style.backgroundImage = `url('${images[currentImageIndex]}')`;
        }, 5000);
    } catch (error) {
        console.error('Error parsing slideshow images:', error);
    }
}

// Smooth Scroll to Top
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// Initialize Mobile Menu Toggle (if needed)
function initializeMobileMenu() {
    const mobileMenuButton = document.getElementById('mobile-menu-button');
    const mobileMenu = document.getElementById('mobile-menu');

    if (!mobileMenuButton || !mobileMenu) return;

    mobileMenuButton.addEventListener('click', () => {
        mobileMenu.classList.toggle('hidden');
    });
}

// Add fade-in animation to elements
function addFadeInAnimation() {
    const elements = document.querySelectorAll('.animate-on-scroll');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1
    });

    elements.forEach(element => observer.observe(element));
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 0
    }).format(amount);
}

// Show notification (can be used for booking confirmations, etc.)
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `fixed top-20 right-4 z-50 px-6 py-4 rounded-lg shadow-lg transition-all duration-300 ${type === 'success' ? 'bg-green-600' :
            type === 'error' ? 'bg-red-600' :
                'bg-blue-600'
        } text-white`;
    notification.textContent = message;

    document.body.appendChild(notification);

    // Fade in
    setTimeout(() => {
        notification.style.opacity = '1';
    }, 10);

    // Remove after 3 seconds
    setTimeout(() => {
        notification.style.opacity = '0';
        setTimeout(() => {
            document.body.removeChild(notification);
        }, 300);
    }, 3000);
}

// Initialize all functions when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    initializeHotelSearch();
    initializeSlideshow();
    initializeMobileMenu();
    addFadeInAnimation();

    // Add scroll to top on logo click
    const logo = document.querySelector('.gradient-text');
    if (logo) {
        logo.parentElement.addEventListener('click', (e) => {
            if (window.location.pathname === '/') {
                e.preventDefault();
                scrollToTop();
            }
        });
    }
});

// Export functions for use in other scripts if needed
window.AuraStays = {
    showNotification,
    formatCurrency,
    scrollToTop
};