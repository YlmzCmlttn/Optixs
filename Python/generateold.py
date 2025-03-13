from PIL import Image, ImageDraw
import math

def create_circle_image(image_size=100, fill_color="gray", border_color="white", line_width=2, output_filename="circle.png"):
    """
    Draws a circle that touches the image edges.
    """
    # Create a new transparent image (RGBA mode)
    image = Image.new("RGBA", (image_size, image_size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)
    
    # For ellipse, using the full image area so the border touches the edges.
    bbox = [0, 0, image_size - 1, image_size - 1]
    draw.ellipse(bbox, fill=fill_color, outline=border_color, width=line_width)
    
    image.save(output_filename, "PNG")


def create_square_image(image_size=100, fill_color="gray", border_color="white", line_width=2, output_filename="square.png"):
    """
    Draws a square that touches the image edges.
    """
    image = Image.new("RGBA", (image_size, image_size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)
    
    # The rectangle's bounding box from top-left (0,0) to bottom-right (image_size-1, image_size-1)
    bbox = [0, 0, image_size - 1, image_size - 1]
    draw.rectangle(bbox, fill=fill_color, outline=border_color, width=line_width)
    
    image.save(output_filename, "PNG")


def line_intersection(line1, line2):
    """
    Returns the intersection point of two lines.
    Each line is given as a pair of points: ((x1, y1), (x2, y2)).
    """
    (x1, y1), (x2, y2) = line1
    (x3, y3), (x4, y4) = line2

    denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
    if denom == 0:
        return (0, 0)  # Lines are parallel; should not occur for a proper triangle.
    t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom
    ix = x1 + t * (x2 - x1)
    iy = y1 + t * (y2 - y1)
    return (ix, iy)


def offset_triangle(vertices, d):
    """
    Offsets the edges of a triangle inward by a distance d.
    Returns the new (inner) triangle vertices.
    """
    # Compute centroid of the triangle
    cx = sum([v[0] for v in vertices]) / 3
    cy = sum([v[1] for v in vertices]) / 3
    centroid = (cx, cy)
    
    offset_lines = []
    n = len(vertices)
    for i in range(n):
        A = vertices[i]
        B = vertices[(i + 1) % n]
        # Edge vector
        dx = B[0] - A[0]
        dy = B[1] - A[1]
        length = math.hypot(dx, dy)
        # Two possible unit normals: (-dy/length, dx/length) and (dy/length, -dx/length)
        n1 = (-dy / length, dx / length)
        n2 = (dy / length, -dx / length)
        # Choose the normal that points inward (toward the centroid)
        midpoint = ((A[0] + B[0]) / 2, (A[1] + B[1]) / 2)
        vec_to_centroid = (centroid[0] - midpoint[0], centroid[1] - midpoint[1])
        dot1 = vec_to_centroid[0] * n1[0] + vec_to_centroid[1] * n1[1]
        dot2 = vec_to_centroid[0] * n2[0] + vec_to_centroid[1] * n2[1]
        chosen_normal = n1 if dot1 > dot2 else n2
        
        # Offset both points of the edge by distance d along the chosen normal
        offset_A = (A[0] + d * chosen_normal[0], A[1] + d * chosen_normal[1])
        offset_B = (B[0] + d * chosen_normal[0], B[1] + d * chosen_normal[1])
        offset_lines.append((offset_A, offset_B))
    
    # Find intersections of consecutive offset lines to determine new vertices.
    new_vertices = []
    for i in range(n):
        line1 = offset_lines[i]
        line2 = offset_lines[(i + 1) % n]
        new_vertices.append(line_intersection(line1, line2))
        
    return new_vertices


def create_triangle_image(image_size=100, fill_color="gray", border_color="white", line_width=2, output_filename="triangle.png"):
    """
    Draws a triangle whose outer border touches the image edges.
    The triangle is drawn by first drawing an outer triangle in the border color,
    then an inner, slightly smaller triangle (inset by line_width) is drawn in the fill color.
    """
    image = Image.new("RGBA", (image_size, image_size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)
    
    # Define outer triangle vertices that touch the image edges.
    # Here we choose an isosceles triangle with the top vertex at the center of the top edge,
    # and the bottom vertices at the bottom-left and bottom-right corners.
    outer_vertices = [(image_size / 2, 0), (0, image_size - 1), (image_size - 1, image_size - 1)]
    
    # Draw the outer triangle (the "border") in the border color.
    draw.polygon(outer_vertices, fill=border_color)
    
    # Compute inner triangle vertices offset inward by the border thickness (line_width)
    inner_vertices = offset_triangle(outer_vertices, line_width)
    draw.polygon(inner_vertices, fill=fill_color)
    
    image.save(output_filename, "PNG")


# Example usage:
create_circle_image(fill_color="gray", border_color="white", line_width=2, output_filename="circle.png")
create_square_image(fill_color="gray", border_color="white", line_width=2, output_filename="square.png")
create_triangle_image(fill_color="gray", border_color="white", line_width=2, output_filename="triangle.png")
