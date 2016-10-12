function [ Image ] = plot_lines(lines, X, Y, angle)

Image = zeros(X, Y);
for k = 1:length(lines)
    xy = [lines(k).point1; lines(k).point2];
    p1 = [xy(1,1) xy(1,2)];
    p2 = [xy(2,1) xy(2,2)];
    
    % Get the equation of the line: y = ax+b
    x = p1(1):p2(1);
    dx = (p2(1) - p1(1)) + 0.0001;
    dy = p2(2) - p1(2);
    y = round((x - p1(1)) * dy / dx + p1(2));
    
    % Convert the indices to be usable with the matrix
    idx = sub2ind(size(Image), y, x);
    Image(idx) = 1;
end
Image = imrotate(Image, angle, 'crop', 'bilinear');
end

