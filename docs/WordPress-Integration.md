# WordPress Iframe Integration Guide

## Overview

This guide provides comprehensive instructions for integrating the LocationFinder application into WordPress sites using iframe embedding. The application is designed to work seamlessly within WordPress while maintaining security and performance.

## 🎯 Integration Methods

### Method 1: Shortcode Integration (Recommended)

#### Create WordPress Shortcode

Add this code to your WordPress theme's `functions.php` file or in a custom plugin:

```php
<?php
/**
 * LocationFinder Shortcode
 * Usage: [locationfinder width="100%" height="600px"]
 */
function locationfinder_shortcode($atts) {
    // Parse attributes
    $atts = shortcode_atts(array(
        'width' => '100%',
        'height' => '600px',
        'api_url' => 'https://your-api-domain.com',
        'theme' => 'default'
    ), $atts);
    
    // Sanitize inputs
    $width = esc_attr($atts['width']);
    $height = esc_attr($atts['height']);
    $api_url = esc_url($atts['api_url']);
    $theme = esc_attr($atts['theme']);
    
    // Generate unique ID for iframe
    $iframe_id = 'locationfinder-' . uniqid();
    
    // Build iframe HTML
    $iframe_html = sprintf(
        '<div class="locationfinder-container" style="width: %s; height: %s; margin: 20px 0;">
            <iframe 
                id="%s"
                src="%s"
                width="100%%"
                height="100%%"
                frameborder="0"
                scrolling="no"
                allow="geolocation"
                title="Location Finder"
                loading="lazy"
                style="border: 1px solid #ddd; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
            </iframe>
            <div class="locationfinder-loading" style="text-align: center; padding: 20px; color: #666;">
                Loading Location Finder...
            </div>
        </div>
        <script>
        document.addEventListener("DOMContentLoaded", function() {
            const iframe = document.getElementById("%s");
            const loading = iframe.parentNode.querySelector(".locationfinder-loading");
            
            iframe.onload = function() {
                loading.style.display = "none";
            };
            
            iframe.onerror = function() {
                loading.innerHTML = "Error loading Location Finder. Please try again.";
            };
        });
        </script>',
        $width,
        $height,
        $iframe_id,
        $api_url,
        $iframe_id
    );
    
    return $iframe_html;
}
add_shortcode('locationfinder', 'locationfinder_shortcode');
?>
```

#### Usage Examples

```php
// Basic usage
[locationfinder]

// Custom dimensions
[locationfinder width="800px" height="500px"]

// Custom API URL
[locationfinder api_url="https://api.yourdomain.com"]

// Custom theme
[locationfinder theme="dark"]
```

### Method 2: Page Template Integration

#### Create Custom Page Template

Create a file named `page-locationfinder.php` in your theme directory:

```php
<?php
/*
Template Name: Location Finder
*/

get_header(); ?>

<div class="locationfinder-page">
    <div class="container">
        <div class="row">
            <div class="col-12">
                <h1><?php the_title(); ?></h1>
                <?php the_content(); ?>
                
                <div class="locationfinder-iframe-container">
                    <iframe 
                        src="https://your-api-domain.com"
                        width="100%"
                        height="700px"
                        frameborder="0"
                        scrolling="no"
                        allow="geolocation"
                        title="Location Finder"
                        loading="lazy"
                        style="border: 1px solid #ddd; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
                    </iframe>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
.locationfinder-page {
    padding: 40px 0;
}

.locationfinder-iframe-container {
    margin: 30px 0;
    position: relative;
}

.locationfinder-iframe-container iframe {
    display: block;
    margin: 0 auto;
}
</style>

<?php get_footer(); ?>
```

### Method 3: Widget Integration

#### Create Custom Widget

```php
<?php
/**
 * LocationFinder Widget
 */
class LocationFinder_Widget extends WP_Widget {
    
    public function __construct() {
        parent::__construct(
            'locationfinder_widget',
            'Location Finder',
            array('description' => 'Embed LocationFinder in your sidebar or footer')
        );
    }
    
    public function widget($args, $instance) {
        echo $args['before_widget'];
        
        if (!empty($instance['title'])) {
            echo $args['before_title'] . apply_filters('widget_title', $instance['title']) . $args['after_title'];
        }
        
        $height = !empty($instance['height']) ? $instance['height'] : '400px';
        $api_url = !empty($instance['api_url']) ? $instance['api_url'] : 'https://your-api-domain.com';
        
        echo sprintf(
            '<div class="locationfinder-widget">
                <iframe 
                    src="%s"
                    width="100%%"
                    height="%s"
                    frameborder="0"
                    scrolling="no"
                    allow="geolocation"
                    title="Location Finder"
                    loading="lazy"
                    style="border: 1px solid #ddd; border-radius: 6px;">
                </iframe>
            </div>',
            esc_url($api_url),
            esc_attr($height)
        );
        
        echo $args['after_widget'];
    }
    
    public function form($instance) {
        $title = !empty($instance['title']) ? $instance['title'] : '';
        $height = !empty($instance['height']) ? $instance['height'] : '400px';
        $api_url = !empty($instance['api_url']) ? $instance['api_url'] : 'https://your-api-domain.com';
        ?>
        <p>
            <label for="<?php echo $this->get_field_id('title'); ?>">Title:</label>
            <input class="widefat" id="<?php echo $this->get_field_id('title'); ?>" 
                   name="<?php echo $this->get_field_name('title'); ?>" type="text" 
                   value="<?php echo esc_attr($title); ?>">
        </p>
        <p>
            <label for="<?php echo $this->get_field_id('height'); ?>">Height:</label>
            <input class="widefat" id="<?php echo $this->get_field_id('height'); ?>" 
                   name="<?php echo $this->get_field_name('height'); ?>" type="text" 
                   value="<?php echo esc_attr($height); ?>">
        </p>
        <p>
            <label for="<?php echo $this->get_field_id('api_url'); ?>">API URL:</label>
            <input class="widefat" id="<?php echo $this->get_field_id('api_url'); ?>" 
                   name="<?php echo $this->get_field_name('api_url'); ?>" type="url" 
                   value="<?php echo esc_attr($api_url); ?>">
        </p>
        <?php
    }
    
    public function update($new_instance, $old_instance) {
        $instance = array();
        $instance['title'] = (!empty($new_instance['title'])) ? strip_tags($new_instance['title']) : '';
        $instance['height'] = (!empty($new_instance['height'])) ? strip_tags($new_instance['height']) : '400px';
        $instance['api_url'] = (!empty($new_instance['api_url'])) ? esc_url_raw($new_instance['api_url']) : '';
        return $instance;
    }
}

// Register the widget
function register_locationfinder_widget() {
    register_widget('LocationFinder_Widget');
}
add_action('widgets_init', 'register_locationfinder_widget');
?>
```

## 🔧 Configuration

### CORS Configuration

Ensure your API's CORS settings include your WordPress domain:

```csharp
// In Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("WordPressIframe", policy =>
    {
        policy.WithOrigins(
                "https://yourwordpresssite.com",
                "https://www.yourwordpresssite.com",
                "http://localhost:8080" // For development
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

### Security Headers

Configure security headers in your `web.config`:

```xml
<httpProtocol>
    <customHeaders>
        <add name="X-Content-Type-Options" value="nosniff" />
        <add name="X-Frame-Options" value="SAMEORIGIN" />
        <add name="X-XSS-Protection" value="1; mode=block" />
        <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
        <add name="Content-Security-Policy" value="frame-ancestors 'self' https://yourwordpresssite.com;" />
    </customHeaders>
</httpProtocol>
```

## 🎨 Styling and Theming

### Custom CSS for WordPress Integration

Add this CSS to your WordPress theme:

```css
/* LocationFinder iframe styling */
.locationfinder-container {
    position: relative;
    margin: 20px 0;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 4px 20px rgba(0,0,0,0.1);
}

.locationfinder-container iframe {
    border: none;
    transition: all 0.3s ease;
}

.locationfinder-container:hover iframe {
    transform: translateY(-2px);
    box-shadow: 0 6px 25px rgba(0,0,0,0.15);
}

/* Loading state */
.locationfinder-loading {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background: rgba(255,255,255,0.9);
    padding: 20px;
    border-radius: 8px;
    z-index: 10;
}

/* Responsive design */
@media (max-width: 768px) {
    .locationfinder-container {
        margin: 10px 0;
    }
    
    .locationfinder-container iframe {
        height: 400px !important;
    }
}

/* Dark theme support */
@media (prefers-color-scheme: dark) {
    .locationfinder-container {
        background: #2d3748;
    }
    
    .locationfinder-loading {
        background: rgba(45, 55, 72, 0.9);
        color: #e2e8f0;
    }
}
```

### WordPress Theme Integration

#### Add to Theme Functions

```php
<?php
// Add LocationFinder styles and scripts
function locationfinder_enqueue_assets() {
    wp_enqueue_style(
        'locationfinder-styles',
        get_template_directory_uri() . '/assets/css/locationfinder.css',
        array(),
        '1.0.0'
    );
    
    wp_enqueue_script(
        'locationfinder-scripts',
        get_template_directory_uri() . '/assets/js/locationfinder.js',
        array('jquery'),
        '1.0.0',
        true
    );
}
add_action('wp_enqueue_scripts', 'locationfinder_enqueue_assets');
?>
```

## 📱 Responsive Design

### Mobile Optimization

```css
/* Mobile-first responsive design */
.locationfinder-container {
    width: 100%;
    max-width: 100%;
    margin: 10px 0;
}

.locationfinder-container iframe {
    width: 100%;
    height: 400px; /* Default mobile height */
}

/* Tablet */
@media (min-width: 768px) {
    .locationfinder-container iframe {
        height: 500px;
    }
}

/* Desktop */
@media (min-width: 1024px) {
    .locationfinder-container iframe {
        height: 600px;
    }
}

/* Large screens */
@media (min-width: 1440px) {
    .locationfinder-container iframe {
        height: 700px;
    }
}
```

## 🔒 Security Considerations

### Content Security Policy

Add CSP headers to prevent XSS attacks:

```php
// Add to functions.php or security plugin
function locationfinder_csp_headers() {
    if (!is_admin()) {
        header("Content-Security-Policy: frame-ancestors 'self' https://yourwordpresssite.com;");
    }
}
add_action('send_headers', 'locationfinder_csp_headers');
```

### Input Validation

```php
// Validate shortcode attributes
function locationfinder_validate_attributes($atts) {
    $validated = array();
    
    // Validate width
    $validated['width'] = preg_match('/^\d+(px|%|em|rem)$/', $atts['width']) 
        ? $atts['width'] : '100%';
    
    // Validate height
    $validated['height'] = preg_match('/^\d+(px|%|em|rem)$/', $atts['height']) 
        ? $atts['height'] : '600px';
    
    // Validate API URL
    $validated['api_url'] = filter_var($atts['api_url'], FILTER_VALIDATE_URL) 
        ? $atts['api_url'] : 'https://your-api-domain.com';
    
    return $validated;
}
```

## 🚀 Performance Optimization

### Lazy Loading

```php
// Implement lazy loading for iframes
function locationfinder_lazy_load_script() {
    ?>
    <script>
    document.addEventListener("DOMContentLoaded", function() {
        const iframes = document.querySelectorAll('.locationfinder-container iframe');
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const iframe = entry.target;
                    iframe.src = iframe.dataset.src;
                    observer.unobserve(iframe);
                }
            });
        });
        
        iframes.forEach(iframe => {
            iframe.dataset.src = iframe.src;
            iframe.src = '';
            observer.observe(iframe);
        });
    });
    </script>
    <?php
}
add_action('wp_footer', 'locationfinder_lazy_load_script');
```

### Caching

```php
// Cache iframe content
function locationfinder_cache_iframe($api_url, $cache_time = 3600) {
    $cache_key = 'locationfinder_' . md5($api_url);
    $cached_content = get_transient($cache_key);
    
    if (false === $cached_content) {
        // Fetch content from API
        $response = wp_remote_get($api_url);
        
        if (!is_wp_error($response)) {
            $cached_content = wp_remote_retrieve_body($response);
            set_transient($cache_key, $cached_content, $cache_time);
        }
    }
    
    return $cached_content;
}
```

## 🧪 Testing

### Cross-Browser Testing

Test the integration across different browsers:

```javascript
// Browser compatibility check
function checkBrowserCompatibility() {
    const iframe = document.querySelector('.locationfinder-container iframe');
    
    if (!iframe) {
        console.warn('LocationFinder iframe not found');
        return;
    }
    
    // Check iframe support
    if (!window.postMessage) {
        console.warn('postMessage not supported');
    }
    
    // Check geolocation support
    if (!navigator.geolocation) {
        console.warn('Geolocation not supported');
    }
}

// Run compatibility check
document.addEventListener('DOMContentLoaded', checkBrowserCompatibility);
```

### Mobile Testing

```css
/* Mobile-specific styles */
@media (max-width: 480px) {
    .locationfinder-container {
        margin: 5px 0;
    }
    
    .locationfinder-container iframe {
        height: 350px;
        font-size: 14px;
    }
    
    /* Hide loading text on very small screens */
    .locationfinder-loading {
        font-size: 12px;
        padding: 10px;
    }
}
```

## 📊 Analytics and Tracking

### Google Analytics Integration

```php
// Track iframe interactions
function locationfinder_analytics() {
    ?>
    <script>
    // Track iframe load
    document.addEventListener('DOMContentLoaded', function() {
        const iframes = document.querySelectorAll('.locationfinder-container iframe');
        
        iframes.forEach(iframe => {
            iframe.addEventListener('load', function() {
                if (typeof gtag !== 'undefined') {
                    gtag('event', 'locationfinder_loaded', {
                        'event_category': 'engagement',
                        'event_label': 'iframe_load'
                    });
                }
            });
        });
    });
    
    // Track iframe interactions
    window.addEventListener('message', function(event) {
        if (event.origin !== 'https://your-api-domain.com') return;
        
        if (event.data.type === 'locationfinder_search') {
            if (typeof gtag !== 'undefined') {
                gtag('event', 'locationfinder_search', {
                    'event_category': 'engagement',
                    'event_label': event.data.zipCode
                });
            }
        }
    });
    </script>
    <?php
}
add_action('wp_footer', 'locationfinder_analytics');
```

## 🔧 Troubleshooting

### Common Issues

#### Iframe Not Loading
```php
// Debug iframe loading issues
function locationfinder_debug() {
    if (current_user_can('administrator')) {
        ?>
        <script>
        console.log('LocationFinder Debug Mode');
        
        const iframes = document.querySelectorAll('.locationfinder-container iframe');
        iframes.forEach((iframe, index) => {
            console.log(`Iframe ${index}:`, {
                src: iframe.src,
                width: iframe.width,
                height: iframe.height,
                style: iframe.style.cssText
            });
        });
        </script>
        <?php
    }
}
add_action('wp_footer', 'locationfinder_debug');
```

#### CORS Issues
```php
// Check CORS configuration
function locationfinder_cors_check() {
    $api_url = 'https://your-api-domain.com';
    $response = wp_remote_get($api_url . '/api/health');
    
    if (is_wp_error($response)) {
        error_log('LocationFinder CORS check failed: ' . $response->get_error_message());
    }
}
```

## 📚 Best Practices

1. **Always use HTTPS** for both WordPress and API
2. **Implement proper CORS** configuration
3. **Use lazy loading** for better performance
4. **Add error handling** for iframe failures
5. **Test across devices** and browsers
6. **Monitor performance** and user interactions
7. **Keep API URLs** in configuration files
8. **Use semantic HTML** for accessibility
9. **Implement proper caching** strategies
10. **Regular security updates** and monitoring

---

**Last Updated**: January 2024  
**Version**: 1.0.0
