from PIL import Image, ImageDraw
import math

def create_triangle_image(img_width, img_height, border_color, fill_color, border_thickness=1):
    """
    Creates a transparent image with an equilateral triangle.
    
    The triangle's base will span the full available width (minus margins for the border),
    and its height is computed accordingly. If the computed height is too tall for the given
    image height, the triangle will be scaled to fit vertically.
    """
    
    
    # Margin to ensure the border is fully visible.
    margin = border_thickness / 2.0
    
    # Use the available width for the triangle's side length.
    effective_width = img_width - 2 * margin
    # For an equilateral triangle, height = side * sqrt(3)/2.
    triangle_height = effective_width * math.sqrt(3) / 2
    img_height = int(img_width * math.sqrt(3) / 2)

    # Create an image with a transparent background.
    img = Image.new("RGBA", (img_width, img_height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # If the triangle's height is too tall for the image, scale it down.
    available_height = img_height - 2 * margin
    if triangle_height > available_height:
        triangle_height = available_height
        effective_width = triangle_height * 2 / math.sqrt(3)
    
    # Center the triangle in the image.
    center_x, center_y = img_width / 2, img_height / 2
    # The triangle's top vertex.
    top_vertex = (center_x, center_y - triangle_height / 2)
    # The triangle's bottom vertices.
    left_vertex = (center_x - effective_width / 2, center_y + triangle_height / 2)
    right_vertex = (center_x + effective_width / 2, center_y + triangle_height / 2)
    
    vertices = [left_vertex, right_vertex, top_vertex]
    
    # Draw filled triangle.
    draw.polygon(vertices, fill=fill_color)
    # Draw the triangle border.
    draw.line(vertices + [vertices[0]], fill=border_color, width=border_thickness)
    
    return img

def create_square_image(img_width, img_height, border_color, fill_color, border_thickness=1):
    """
    Creates a transparent image with a square centered in it.
    """
    img = Image.new("RGBA", (img_width, img_height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    margin = border_thickness / 2.0
    # Define a square that uses the full width (and is centered vertically).
    bbox = [margin, margin, img_width - margin, img_height - margin]
    
    draw.rectangle(bbox, fill=fill_color)
    draw.rectangle(bbox, outline=border_color, width=border_thickness)
    
    return img

def create_circle_image(img_width, img_height, border_color, fill_color, border_thickness=1):
    """
    Creates a transparent image with a circle centered in it.
    """
    img = Image.new("RGBA", (img_width, img_height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    margin = border_thickness / 2.0
    bbox = [margin, margin, img_width - margin, img_height - margin]
    
    draw.ellipse(bbox, fill=fill_color, outline=border_color, width=border_thickness)
    
    return img

# Example usage:
if __name__ == '__main__':
    # Set the image resolution.
    width, height = 256, 256
    
    # Define border and fill colors in RGBA.
    fill_color = (64, 64, 64, 64)   # Red border
    border_color   = (255, 255, 255, 255)     # Green fill
    border_thickness = 5
    
    # Generate texture images.
    triangle_img = create_triangle_image(width, height, border_color, fill_color, border_thickness)
    square_img   = create_square_image(width, height, border_color, fill_color, border_thickness)
    circle_img   = create_circle_image(width, height, border_color, fill_color, border_thickness)
    
    # Save the images or use them directly in your game engine.
    triangle_img.save("triangle_texture.png")
    square_img.save("square_texture.png")
    circle_img.save("circle_texture.png")
