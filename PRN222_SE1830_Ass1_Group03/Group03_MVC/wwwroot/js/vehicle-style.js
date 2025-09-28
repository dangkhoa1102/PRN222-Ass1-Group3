/* Vehicle Management Custom Styles - Enhanced Version */

/* Animation keyframes */
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(20px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@keyframes slideInFromRight {
    from {
        opacity: 0;
        transform: translateX(30px);
    }
    to {
        opacity: 1;
        transform: translateX(0);
    }
}

@keyframes pulse {
    0 %, 100 % { opacity: 1; }
    50 % { opacity: 0.7; }
}

/* Card hover effects */
.hover - shadow {
    transition: all 0.3s cubic - bezier(0.4, 0, 0.2, 1);
    cursor: pointer;
}

.hover - shadow:hover {
    transform: translateY(-8px) scale(1.02);
    box - shadow: 0 1.5rem 4rem rgba(0, 0, 0, 0.15)!important;
}

/* Enhanced vehicle card styling */
.vehicle - card {
    transition: all 0.3s ease;
}

.vehicle - card.card {
    border: 1px solid #e9ecef;
    border - radius: 1rem;
    overflow: hidden;
    background: linear - gradient(135deg, #ffffff 0 %, #f8f9fa 100 %);
}

.vehicle - card: hover.card {
    border - color: #007bff;
    box - shadow: 0 0 0 0.1rem rgba(0, 123, 255, 0.1);
}

/* Enhanced image styling */
.vehicle - image {
    transition: all 0.4s cubic - bezier(0.4, 0, 0.2, 1);
    filter: brightness(1) contrast(1);
}

.card: hover.vehicle - image {
    transform: scale(1.08);
    filter: brightness(1.1) contrast(1.1);
}

.vehicle - image - container {
    position: relative;
    overflow: hidden;
    border - radius: 1rem 1rem 0 0;
}

.vehicle - image - container::after {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear - gradient(180deg, transparent 60 %, rgba(0, 0, 0, 0.1) 100 %);
    pointer - events: none;
    transition: opacity 0.3s ease;
}

.card: hover.vehicle - image - container::after {
    opacity: 0;
}

/* Quick actions overlay */
.quick - actions {
    backdrop - filter: blur(10px);
    border - radius: 0.5rem;
    padding: 0.5rem;
    background: rgba(255, 255, 255, 0.9);
}

.quick - actions.btn {
    backdrop - filter: blur(5px);
    border: 1px solid rgba(255, 255, 255, 0.3);
}

/* Enhanced badge styling */
.badge {
    font - size: 0.7rem;
    font - weight: 600;
    padding: 0.4em 0.8em;
    border - radius: 2rem;
    backdrop - filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
}

.badge.bg - success {
    background: linear - gradient(135deg, #28a745 0 %, #20c997 100 %)!important;
    box - shadow: 0 2px 4px rgba(40, 167, 69, 0.3);
}

.badge.bg - warning {
    background: linear - gradient(135deg, #ffc107 0 %, #fd7e14 100 %)!important;
    box - shadow: 0 2px 4px rgba(255, 193, 7, 0.3);
    color: #212529!important;
}

.badge.bg - danger {
    background: linear - gradient(135deg, #dc3545 0 %, #e83e8c 100 %)!important;
    box - shadow: 0 2px 4px rgba(220, 53, 69, 0.3);
}

/* Enhanced form controls */
.form - control, .form - select {
    border: 2px solid #e9ecef;
    border - radius: 0.75rem;
    padding: 0.75rem 1rem;
    transition: all 0.3s ease;
    background: rgba(255, 255, 255, 0.8);
}

.form - control: focus, .form - select:focus {
    border - color: #007bff;
    box - shadow: 0 0 0 0.3rem rgba(0, 123, 255, 0.15);
    background: rgba(255, 255, 255, 1);
    transform: translateY(-1px);
}

.form - control: hover, .form - select:hover {
    border - color: #80bdff;
    background: rgba(255, 255, 255, 0.9);
}

/* Enhanced button styles */
.btn {
    border - radius: 0.75rem;
    font - weight: 500;
    padding: 0.5rem 1.25rem;
    transition: all 0.3s cubic - bezier(0.4, 0, 0.2, 1);
    position: relative;
    overflow: hidden;
}

.btn:before {
    content: '';
    position: absolute;
    top: 0;
    left: -100 %;
    width: 100 %;
    height: 100 %;
    background: linear - gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
    transition: left 0.5s;
}

.btn: hover:before {
    left: 100 %;
}

.btn - primary {
    background: linear - gradient(135deg, #007bff 0 %, #0056b3 100 %);
    border: none;
    box - shadow: 0 4px 12px rgba(0, 123, 255, 0.3);
}

.btn - primary:hover {
    transform: translateY(-2px);
    box - shadow: 0 6px 16px rgba(0, 123, 255, 0.4);
}

.btn - success {
    background: linear - gradient(135deg, #28a745 0 %, #20c997 100 %);
    border: none;
    box - shadow: 0 4px 12px rgba(40, 167, 69, 0.3);
}

.btn - warning {
    background: linear - gradient(135deg, #ffc107 0 %, #fd7e14 100 %);
    border: none;
    color: #212529;
    box - shadow: 0 4px 12px rgba(255, 193, 7, 0.3);
}

.btn - danger {
    background: linear - gradient(135deg, #dc3545 0 %, #c82333 100 %);
    border: none;
    box - shadow: 0 4px 12px rgba(220, 53, 69, 0.3);
}

.btn - outline - primary:hover {
    background: linear - gradient(135deg, #007bff 0 %, #0056b3 100 %);
    transform: translateY(-1px);
}

/* Enhanced alert styling */
.alert {
    border - radius: 1rem;
    border: none;
    border - left: 4px solid;
    backdrop - filter: blur(10px);
    position: relative;
    overflow: hidden;
}

.alert::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 1px;
    background: linear - gradient(90deg, transparent, rgba(255, 255, 255, 0.5), transparent);
}

.alert - success {
    background: linear - gradient(135deg, rgba(40, 167, 69, 0.1) 0 %, rgba(32, 201, 151, 0.1) 100 %);
    border - left - color: #28a745;
}

.alert - danger {
    background: linear - gradient(135deg, rgba(220, 53, 69, 0.1) 0 %, rgba(200, 35, 51, 0.1) 100 %);
    border - left - color: #dc3545;
}

.alert - info {
    background: linear - gradient(135deg, rgba(13, 202, 240, 0.1) 0 %, rgba(0, 123, 255, 0.1) 100 %);
    border - left - color: #0dcaf0;
}

.alert - warning {
    background: linear - gradient(135deg, rgba(255, 193, 7, 0.1) 0 %, rgba(253, 126, 20, 0.1) 100 %);
    border - left - color: #ffc107;
    color: #856404;
}

/* Enhanced modal styling */
.modal - content {
    border - radius: 1.5rem;
    border: none;
    box - shadow: 0 2rem 4rem rgba(0, 0, 0, 0.2);
    backdrop - filter: blur(20px);
    background: rgba(255, 255, 255, 0.98);
}

.modal - header {
    border - bottom: 1px solid rgba(0, 0, 0, 0.1);
    padding: 1.5rem 2rem;
    background: linear - gradient(135deg, rgba(0, 123, 255, 0.05) 0 %, rgba(0, 86, 179, 0.05) 100 %);
    border - radius: 1.5rem 1.5rem 0 0;
}

.modal - body {
    padding: 2rem;
}

.modal - footer {
    border - top: 1px solid rgba(0, 0, 0, 0