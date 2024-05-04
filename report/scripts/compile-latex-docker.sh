# Be careful about line endings! If the file uses CRLF, bash might not be able to run the script
# Inspired by https://gist.github.com/avborup/580bc0b003a702d1a84235890acd5fd7
# How to use script: ./compile-latex-docker.sh main.tex
mkdir -p ./build

docker run \
  --rm \
  -i \
  --net=none \
  -v $(pwd):/data \
  kongborup/custom-latex:latest \
  pdflatex \
  --shell-escape \
  -interaction=nonstopmode \
  -file-line-error \
  -output-directory=build \
  -aux-directory=build \
  $1

docker run --rm -i --net=none -v $(pwd):/data kongborup/custom-latex:latest biber build/main.bcf

docker run \
  --rm \
  -i \
  --net=none \
  -v $(pwd):/data \
  kongborup/custom-latex:latest \
  pdflatex \
  --shell-escape \
  -interaction=nonstopmode \
  -file-line-error \
  -output-directory=build \
  -aux-directory=build \
  $1
